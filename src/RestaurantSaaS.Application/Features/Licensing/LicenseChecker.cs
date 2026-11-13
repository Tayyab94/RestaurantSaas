using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Licensing;

internal sealed class LicenseStateCache
{
    private readonly object _lock = new();
    private LicenseStatusDto? _status;
    private LicenseCheckResult? _result;
    private DateTime _checkedAt;

    public LicenseCheckResult? LastResult { get { lock (_lock) { return _result; } } }

    public void Set(LicenseStatusDto status, LicenseCheckResult result, DateTime now)
    {
        lock (_lock) { _status = status; _result = result; _checkedAt = now; }
    }

    public LicenseStatusDto? Get(DateTime now)
    {
        lock (_lock)
        {
            return _status is not null && now - _checkedAt < TimeSpan.FromMinutes(LicensePolicy.CacheMinutes) && now >= _checkedAt
                ? _status : null;
        }
    }

    public void Clear() { lock (_lock) { _status = null; } }
}

internal sealed class LicenseChecker(
    IApplicationDbContext db, ILicenseFileStore files, ILicenseKeyRing keys,
    IMachineFingerprint machine, IClock clock, LicenseStateCache cache)
{
    private sealed record Outcome(LicenseStatusDto Status, LicenseCheckResult Result, string? Reason, LicensePayload? Payload, string? SignatureHash);

    public async Task<LicenseStatusDto> CheckAsync(LicenseCheckTrigger trigger, CancellationToken ct)
    {
        var now = clock.UtcNow;
        var tenantId = await db.Tenants.IgnoreQueryFilters()
            .Where(t => !t.IsDeleted).Select(t => (Guid?)t.Id).FirstOrDefaultAsync(ct);

        var outcome = await EvaluateAsync(now, tenantId, ct);

        if (tenantId.HasValue && (trigger == LicenseCheckTrigger.Startup || cache.LastResult != outcome.Result))
        {
            db.Set<LicenseCheckLog>().Add(new LicenseCheckLog
            {
                TenantId = tenantId.Value,
                CheckedAt = now,
                Trigger = trigger,
                Result = outcome.Result,
                ReasonCode = outcome.Reason,
                FileHash = outcome.SignatureHash,
                RecordSequence = outcome.Payload?.Sequence,
            });
        }

        await db.SaveChangesAsync(ct);
        cache.Set(outcome.Status, outcome.Result, now);
        return outcome.Status;
    }

    private static Outcome Bad(LicenseState state, LicenseCheckResult result, string reason) =>
        new(LicenseStatuses.Simple(state), result, reason, null, null);

    private async Task<Outcome> EvaluateAsync(DateTime now, Guid? tenantId, CancellationToken ct)
    {
        var records = await db.Set<LicenseRecord>().IgnoreQueryFilters()
            .Where(r => !r.IsDeleted).OrderBy(r => r.Sequence).ToListAsync(ct);

        if (!ChainIsValid(records))
        {
            return Bad(LicenseState.Tampered, LicenseCheckResult.Tampered, "record_chain");
        }

        var content = files.Read();
        if (string.IsNullOrWhiteSpace(content))
        {
            return Bad(LicenseState.NotActivated, LicenseCheckResult.NotActivated, records.Count > 0 ? "file_missing" : "no_file");
        }

        if (!LicenseFileCodec.TryOpen(content, keys, out var payload, out var signatureHash, out var reason))
        {
            return new Outcome(LicenseStatuses.Simple(LicenseState.Tampered), LicenseCheckResult.Tampered, reason, null, null);
        }

        var me = machine.Get();
        if (!FingerprintMatcher.Matches(payload.Fingerprint, payload.FingerprintParts, me.Hash, me.Parts))
        {
            return new Outcome(LicenseStatuses.Simple(LicenseState.Mismatch), LicenseCheckResult.FingerprintMismatch, "other_pc", payload, signatureHash);
        }

        var last = records.LastOrDefault();
        if (last is not null)
        {
            if (payload.Sequence < last.Sequence)
            {
                return new Outcome(LicenseStatuses.Simple(LicenseState.Mismatch), LicenseCheckResult.Mismatch, "older_file", payload, signatureHash);
            }

            if (payload.Sequence == last.Sequence && signatureHash != last.SignatureHash)
            {
                return new Outcome(LicenseStatuses.Simple(LicenseState.Tampered), LicenseCheckResult.Tampered, "same_sequence_other_signature", payload, signatureHash);
            }
        }

        var clockProblem = await CheckClockAsync(now, tenantId, payload, ct);
        if (clockProblem is not null)
        {
            return new Outcome(LicenseStatuses.Simple(clockProblem.Value),
                clockProblem == LicenseState.ClockError ? LicenseCheckResult.ClockError : LicenseCheckResult.Tampered,
                clockProblem == LicenseState.ClockError ? "clock_behind" : "clock_mark", payload, signatureHash);
        }

        var repaired = false;
        if (tenantId.HasValue && (last is null || payload.Sequence > last.Sequence))
        {
            AddRecord(tenantId.Value, payload, signatureHash, content, last, now);
            repaired = last is not null && last.LicenseId == payload.LicenseId;
        }

        var type = Enum.TryParse<LicenseType>(payload.LicenseType, true, out var parsed) ? parsed : LicenseType.Trial;
        var evaluation = LicenseEvaluator.Evaluate(type, payload.ExpiresAt, now);
        var status = LicenseStatuses.Build(
            evaluation.State, type, payload.PlanCode, payload.LicensedTo, payload.KeyHint,
            payload.ExpiresAt, evaluation.DaysLeft, evaluation.GraceDaysLeft, payload.MaxDevices, payload.MaxBranches);

        return new Outcome(status, repaired ? LicenseCheckResult.Repaired : LicenseCheckResult.Ok, null, payload, signatureHash);
    }

    private async Task<LicenseState?> CheckClockAsync(DateTime now, Guid? tenantId, LicensePayload payload, CancellationToken ct)
    {
        var floor = payload.IssuedAt.AddHours(-24);

        var lastActivity = await db.Set<LoginHistory>().IgnoreQueryFilters().MaxAsync(l => (DateTime?)l.AttemptedAt, ct);
        if (lastActivity is { } activity && activity > floor)
        {
            floor = activity;
        }

        LicenseClockMark? mark = null;
        if (tenantId.HasValue)
        {
            mark = await db.Set<LicenseClockMark>().IgnoreQueryFilters()
                .Where(m => !m.IsDeleted && m.TenantId == tenantId.Value)
                .OrderByDescending(m => m.SeenAtUtc).FirstOrDefaultAsync(ct);

            if (mark is not null)
            {
                if (mark.Hmac != MarkHmac(payload.Fingerprint, tenantId.Value, mark.SeenAtUtc))
                {
                    return LicenseState.Tampered;
                }

                if (mark.SeenAtUtc > floor)
                {
                    floor = mark.SeenAtUtc;
                }
            }
        }

        if (now < floor.AddMinutes(-LicensePolicy.ClockToleranceMinutes))
        {
            return LicenseState.ClockError;
        }

        if (tenantId.HasValue)
        {
            if (mark is null)
            {
                db.Set<LicenseClockMark>().Add(new LicenseClockMark
                {
                    TenantId = tenantId.Value,
                    SeenAtUtc = now,
                    LastBusinessRecordAt = lastActivity,
                    Source = LicenseClockSource.Local,
                    Hmac = MarkHmac(payload.Fingerprint, tenantId.Value, now),
                });
            }
            else if (now > mark.SeenAtUtc.AddMinutes(5))
            {
                mark.SeenAtUtc = now;
                mark.LastBusinessRecordAt = lastActivity;
                mark.Hmac = MarkHmac(payload.Fingerprint, tenantId.Value, now);
            }
        }

        return null;
    }

    private static string MarkHmac(string fingerprint, Guid tenantId, DateTime seenAt) =>
        LicenseHashing.Hmac(RecordKey(fingerprint), $"mark|{tenantId:N}|{seenAt.Ticks}");

    private void AddRecord(Guid tenantId, LicensePayload payload, string signatureHash, string content, LicenseRecord? last, DateTime now)
    {
        var envelope = LicenseFileCodec.ReadEnvelope(content);
        var type = Enum.TryParse<LicenseType>(payload.LicenseType, true, out var parsed) ? parsed : LicenseType.Trial;

        var record = new LicenseRecord
        {
            TenantId = tenantId,
            Sequence = payload.Sequence,
            LicenseId = payload.LicenseId,
            KeyHash = payload.KeyHash,
            KeyHint = payload.KeyHint,
            LicenseType = type,
            PlanCode = payload.PlanCode,
            LicensedUserName = payload.LicensedTo,
            MachineFingerprintHash = payload.Fingerprint,
            IssuedAt = payload.IssuedAt,
            ActivatedAt = payload.ActivatedAt,
            ExpiresAt = payload.ExpiresAt,
            MaxDevices = payload.MaxDevices,
            MaxBranches = payload.MaxBranches,
            FeaturesJson = payload.FeaturesJson,
            SignatureHash = signatureHash,
            SignatureKeyId = payload.KeyId,
            SignedPayload = envelope?.Payload ?? string.Empty,
            Signature = envelope?.Signature ?? string.Empty,
            Source = type == LicenseType.Trial
                ? LicenseRecordSource.Trial
                : last is not null && last.LicenseId == payload.LicenseId ? LicenseRecordSource.Renewal : LicenseRecordSource.Online,
            LastValidatedAt = now,
            PreviousRecordHash = last?.RecordHash ?? string.Empty,
        };
        record.RecordHash = ComputeHash(record);
        record.RecordHmac = LicenseHashing.Hmac(RecordKey(record.MachineFingerprintHash), record.RecordHash);
        db.Set<LicenseRecord>().Add(record);
    }

    private static bool ChainIsValid(IReadOnlyList<LicenseRecord> records)
    {
        var previous = string.Empty;
        foreach (var record in records)
        {
            if (record.PreviousRecordHash != previous
                || record.RecordHash != ComputeHash(record)
                || record.RecordHmac != LicenseHashing.Hmac(RecordKey(record.MachineFingerprintHash), record.RecordHash))
            {
                return false;
            }

            previous = record.RecordHash;
        }

        return true;
    }

    private static string ComputeHash(LicenseRecord r) =>
        LicenseHashing.Sha256(string.Join('|',
            r.PreviousRecordHash, r.Sequence, r.LicenseId, r.KeyHash, r.LicenseType,
            r.ExpiresAt?.ToString("O") ?? "-", r.MachineFingerprintHash, r.SignatureHash));

    private static string RecordKey(string fingerprint) => "restopos-record|" + fingerprint;
}
