using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Licensing;

internal sealed class LicenseServerService(
    IApplicationDbContext db, ILicenseSigner signer,
    ILicenseKeyRing keys,          // ADD
    IClock clock, IRequestInfo requestInfo, LicenseOptions options)
{
    private const string DefaultPlanCode = "STANDARD";

    private string Pepper => string.IsNullOrWhiteSpace(options.KeyPepper)
        ? throw new InvalidOperationException("License:KeyPepper is not configured on this License Server.")
        : options.KeyPepper;

    public string HashKey(string normalizedKey) => LicenseHashing.Hmac(Pepper, "key|" + normalizedKey);

    private static RequestRejectedException InvalidKey() =>
        new("key_invalid", "This license key is not valid. Check it and try again.");

    // ---- keys
    public async Task<GeneratedLicenseKeysDto> GenerateKeysAsync(GenerateLicenseKeysRequest request, Guid? generatedBy, CancellationToken ct)
    {
        if (!Enum.TryParse<LicenseType>(request.LicenseType, true, out var type) || type == LicenseType.Trial)
        {
            throw new RequestRejectedException("invalid_type", "Choose 6 months, 12 months or lifetime. Trials are started by the app.");
        }

        var plan = await db.Set<Plan>().FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive, ct)
                   ?? throw new RequestRejectedException("plan_not_found", "The plan was not found.");

        var now = clock.UtcNow;
        var batch = new LicenseBatch
        {
            BatchNumber = $"B{now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            LicenseType = type,
            PlanId = plan.Id,
            Quantity = request.Quantity,
            MaxActivations = request.MaxActivations,
            MaxDevices = request.MaxDevices,
            MaxBranches = request.MaxBranches,
            GeneratedBy = generatedBy ?? Guid.Empty,
            GeneratedAt = now,
            Notes = request.Notes,
        };
        db.Set<LicenseBatch>().Add(batch);

        var keys = new List<string>(request.Quantity);
        for (var i = 0; i < request.Quantity; i++)
        {
            var key = LicenseKeyFormat.Generate();
            var license = new License
            {
                BatchId = batch.Id,
                KeyHash = HashKey(key),
                KeyHint = LicenseKeyFormat.Hint(key),
                KeyChecksum = key[^4..],
                LicenseType = type,
                DurationMonths = type == LicenseType.Months6 ? 6 : type == LicenseType.Months12 ? 12 : null,
                PlanId = plan.Id,
                MaxActivations = request.MaxActivations,
                MaxDevices = request.MaxDevices,
                MaxBranches = request.MaxBranches,
                FeaturesJson = "{}",
                Status = LicenseStatus.Unused,
                SoldPrice = request.SoldPrice,
                Notes = request.Notes,
                Version = 1,
                KeyPurpose = LicenseKeyPurpose.New,
            };
            db.Set<License>().Add(license);
            AddEvent(LicenseEventType.Generated, "ok", null, license.Id, null, null, null);
            keys.Add(LicenseKeyFormat.Display(key));
        }

        await db.SaveChangesAsync(ct);
        return new GeneratedLicenseKeysDto(batch.BatchNumber, keys);
    }

    // ---- desktop and on-premise activation
    public async Task<LicenseIssueDto> ActivateAsync(ServerActivateRequest request, CancellationToken ct)
    {
        var license = await FindUsableLicenseAsync(request.Key, null, ct);
        var now = clock.UtcNow;
        var fingerprint = LicenseHashing.Hmac(Pepper, "fp|" + request.Fingerprint);
        var parts = request.FingerprintParts.Select(p => LicenseHashing.Hmac(Pepper, "part|" + p)).ToArray();

        var activations = await db.Set<LicenseActivation>()
            .Where(a => a.LicenseId == license.Id && a.Status == LicenseActivationStatus.Active)
            .ToListAsync(ct);

        var activation = activations.FirstOrDefault(a => FingerprintMatcher.Matches(
            a.MachineFingerprintHash ?? string.Empty, ReadParts(a.FingerprintPartsHashJson), fingerprint, parts));

        if (activation is null)
        {
            if (license.ActivationCount >= license.MaxActivations)
            {
                AddEvent(LicenseEventType.ActivationFailed, "failed", "key_in_use", license.Id, null, null, fingerprint);
                await db.SaveChangesAsync(ct);
                throw new RequestRejectedException("key_in_use", "This license key is already used on another PC. Contact support to move it.");
            }

            activation = new LicenseActivation
            {
                LicenseId = license.Id,
                MachineFingerprintHash = fingerprint,
                FingerprintPartsHashJson = JsonSerializer.Serialize(parts),
                MachineName = Trim(request.MachineName, 200),
                OsVersion = Trim(request.OsVersion, 200),
                AppVersion = Trim(request.AppVersion, 50),
                DeploymentMode = ParseMode(request.DeploymentMode),
                ActivatedUserName = Trim(request.UserName, 200)!,
                ActivationMethod = LicenseActivationActivationMethod.Online,
                ActivatedAt = now,
                LastIpAddress = requestInfo.IpAddress,
                Status = LicenseActivationStatus.Active,
            };
            db.Set<LicenseActivation>().Add(activation);
            license.ActivationCount++;
            await ConvertTrialAsync(fingerprint, license.Id, ct);
        }

        if (license.Status == LicenseStatus.Unused)
        {
            license.Status = LicenseStatus.Active;
            license.FirstActivatedAt = now;
            license.LicensedUserName = Trim(request.UserName, 200);
            license.ExpiresAt = LicensePolicy.ExpiryFor(license.LicenseType, license.DurationMonths, license.DurationDays, now);
        }

        activation.LastValidatedAt = now;
        activation.ExpiresAt = license.ExpiresAt;
        license.LastValidatedAt = now;

        var plan = await db.Set<Plan>().FirstAsync(p => p.Id == license.PlanId, ct);
        var file = await IssueAsync(license, activation, plan, request.Fingerprint, request.FingerprintParts.ToArray(), ct);

        AddEvent(LicenseEventType.Activated, "ok", null, license.Id, activation.Id, null, fingerprint);
        await RecordAttemptAsync(license.KeyHash, true, ct, save: false);
        await db.SaveChangesAsync(ct);

        return new LicenseIssueDto(file);
    }

    public async Task<LicenseIssueDto> StartTrialAsync(ServerTrialRequest request, CancellationToken ct)
    {
        if (!PhoneNumber.TryNormalize(request.Phone, out var phone))
        {
            throw new RequestRejectedException("invalid_phone", "Enter a valid phone number, for example 0300 1234567.");
        }

        var now = clock.UtcNow;
        var fingerprint = LicenseHashing.Hmac(Pepper, "fp|" + request.Fingerprint);
        var parts = request.FingerprintParts.Select(p => LicenseHashing.Hmac(Pepper, "part|" + p)).ToArray();
        var phoneHash = LicenseHashing.Hmac(Pepper, "phone|" + phone);
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLowerInvariant();
        var emailHash = email is null ? null : LicenseHashing.Hmac(Pepper, "email|" + email);

        var used = await db.Set<TrialRegistry>().AnyAsync(
            t => t.MachineFingerprintHash == fingerprint || t.PhoneHash == phoneHash || (emailHash != null && t.EmailHash == emailHash), ct);

        if (used)
        {
            AddEvent(LicenseEventType.ActivationFailed, "failed", "trial_used", null, null, null, fingerprint);
            await db.SaveChangesAsync(ct);
            throw new RequestRejectedException("trial_used", "A free trial was already used on this PC, phone number or email. Please buy a license key.");
        }

        var plan = await DefaultPlanAsync(ct);
        var license = new License
        {
            KeyHash = LicenseHashing.Hmac(Pepper, "trial|" + Guid.NewGuid().ToString("N")),
            KeyHint = "TRIAL",
            KeyChecksum = string.Empty,
            LicenseType = LicenseType.Trial,
            DurationDays = LicensePolicy.TrialDays,
            PlanId = plan.Id,
            MaxActivations = 1,
            ActivationCount = 1,
            MaxDevices = plan.MaxDevices,
            MaxBranches = plan.MaxBranches,
            FeaturesJson = "{}",
            Status = LicenseStatus.Active,
            LicensedUserName = Trim(request.UserName, 200),
            LicensedPhone = Trim(phone, 30),
            FirstActivatedAt = now,
            ExpiresAt = now.AddDays(LicensePolicy.TrialDays),
            LastValidatedAt = now,
            Version = 1,
            KeyPurpose = LicenseKeyPurpose.New,
        };
        db.Set<License>().Add(license);

        var activation = new LicenseActivation
        {
            LicenseId = license.Id,
            MachineFingerprintHash = fingerprint,
            FingerprintPartsHashJson = JsonSerializer.Serialize(parts),
            MachineName = Trim(request.MachineName, 200),
            OsVersion = Trim(request.OsVersion, 200),
            AppVersion = Trim(request.AppVersion, 50),
            DeploymentMode = ParseMode(request.DeploymentMode),
            ActivatedUserName = Trim(request.UserName, 200)!,
            ActivationMethod = LicenseActivationActivationMethod.Trial,
            ActivatedAt = now,
            LastValidatedAt = now,
            LastIpAddress = requestInfo.IpAddress,
            ExpiresAt = license.ExpiresAt,
            Status = LicenseActivationStatus.Active,
        };
        db.Set<LicenseActivation>().Add(activation);

        db.Set<TrialRegistry>().Add(new TrialRegistry
        {
            MachineFingerprintHash = fingerprint,
            PhoneHash = phoneHash,
            EmailHash = emailHash,
            LicenseId = license.Id,
            StartedAt = now,
            ExpiresAt = license.ExpiresAt.Value,
        });

        var file = await IssueAsync(license, activation, plan, request.Fingerprint, request.FingerprintParts.ToArray(), ct);
        AddEvent(LicenseEventType.TrialStarted, "ok", null, license.Id, activation.Id, null, fingerprint);
        await db.SaveChangesAsync(ct);

        return new LicenseIssueDto(file);
    }


    /// The owner frees this PC. The signed license file is the proof: it must carry our signature
    /// and match the activation. The key keeps its end date, so it can be activated again.
    /// Trials cannot be freed (that would allow a second trial).
    public async Task DeactivateAsync(ServerDeactivateRequest request, CancellationToken ct)
    {
        if (!LicenseFileCodec.TryOpen(request.LicenseFile, keys, out var payload, out _, out _))
        {
            throw new RequestRejectedException("license_file_invalid", "The license file is not valid, so it cannot be deactivated. Please contact support.");
        }

        var license = await db.Set<License>().FirstOrDefaultAsync(l => l.Id == payload.LicenseId, ct)
                      ?? throw new RequestRejectedException("license_file_invalid", "The license file is not valid, so it cannot be deactivated. Please contact support.");

        //if (license.LicenseType == LicenseType.Trial)
        //{
        //    throw new RequestRejectedException("trial_cannot_deactivate", "A free trial cannot be deactivated. Enter a license key to continue.");
        //}

        var activation = await db.Set<LicenseActivation>()
            .FirstOrDefaultAsync(a => a.Id == payload.ActivationId && a.LicenseId == license.Id, ct)
            ?? throw new RequestRejectedException("license_file_invalid", "The license file does not match any activation. Please contact support.");

        // The file must belong to this activation (same PC fingerprint as when it was activated).
        if (activation.MachineFingerprintHash != LicenseHashing.Hmac(Pepper, "fp|" + payload.Fingerprint))
        {
            throw new RequestRejectedException("license_file_invalid", "The license file does not match this activation. Please contact support.");
        }

        // Already freed (for example the file could not be deleted last time): nothing more to do.
        if (activation.Status != LicenseActivationStatus.Active)
        {
            return;
        }

        var now = clock.UtcNow;
        activation.Status = LicenseActivationStatus.Deactivated;
        activation.DeactivatedAt = now;
        activation.DeactivationReason = Trim(request.Reason, 200) ?? "owner";
        license.ActivationCount = Math.Max(0, license.ActivationCount - 1);

        AddEvent(LicenseEventType.Transferred, "ok", "deactivated_by_owner", license.Id, activation.Id, null, activation.MachineFingerprintHash);
        await db.SaveChangesAsync(ct);
    }

    /// The platform administrator frees every PC of a key (lost or broken PC).
    public async Task ResetKeyAsync(Guid licenseId, Guid? resetBy, CancellationToken ct)
    {
        var license = await db.Set<License>().FirstOrDefaultAsync(l => l.Id == licenseId, ct)
                      ?? throw new RequestRejectedException("key_not_found", "The license key was not found.");

        if (license.LicenseType == LicenseType.Trial)
        {
            throw new RequestRejectedException("trial_cannot_deactivate", "A free trial cannot be reset.");
        }

        var active = await db.Set<LicenseActivation>()
            .Where(a => a.LicenseId == license.Id && a.Status == LicenseActivationStatus.Active)
            .ToListAsync(ct);

        var now = clock.UtcNow;
        foreach (var activation in active)
        {
            activation.Status = LicenseActivationStatus.Deactivated;
            activation.DeactivatedAt = now;
            activation.DeactivatedBy = resetBy;
            activation.DeactivationReason = "admin_reset";
            AddEvent(LicenseEventType.Transferred, "ok", "admin_reset", license.Id, activation.Id, null, activation.MachineFingerprintHash);
        }

        license.ActivationCount = 0;
        await db.SaveChangesAsync(ct);
    }

    // ---- cloud activation
    public async Task ActivateForTenantAsync(string key, string userName, Guid tenantId, Guid? userId, CancellationToken ct)
    {
        var license = await FindUsableLicenseAsync(key, tenantId, ct);
        var now = clock.UtcNow;

        var current = await db.Set<Subscription>().Where(s => s.Status == SubscriptionStatus.Active).ToListAsync(ct);

        var start = now;
        foreach (var running in current.Where(s => s.EndDate.HasValue))
        {
            var ends = running.EndDate!.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            if (ends > start)
            {
                start = ends;
            }
        }

        var expires = LicensePolicy.ExpiryFor(license.LicenseType, license.DurationMonths, license.DurationDays, start);

        license.TenantId = tenantId;
        license.Status = LicenseStatus.Active;
        license.ActivationCount = 1;
        license.FirstActivatedAt ??= now;
        license.LicensedUserName = Trim(userName, 200);
        license.ExpiresAt = expires;
        license.LastValidatedAt = now;

        foreach (var running in current)
        {
            running.Status = SubscriptionStatus.Expired;
        }

        db.Set<Subscription>().Add(new Subscription
        {
            TenantId = tenantId,
            PlanId = license.PlanId,
            LicenseId = license.Id,
            LicenseType = license.LicenseType,
            StartDate = DateOnly.FromDateTime(start),
            EndDate = expires is null ? null : DateOnly.FromDateTime(expires.Value),
            BillingCycle = SubscriptionBillingCycle.OneTime,
            Status = SubscriptionStatus.Active,
            AutoRenew = false,
        });

        var activation = new LicenseActivation
        {
            LicenseId = license.Id,
            TenantId = tenantId,
            DeploymentMode = DeploymentMode.Cloud,
            ActivatedUserName = Trim(userName, 200)!,
            ActivationMethod = LicenseActivationActivationMethod.Web,
            ActivatedAt = now,
            LastValidatedAt = now,
            LastIpAddress = requestInfo.IpAddress,
            IssuedSequence = UnixMs(now),
            IssuedSignatureHash = string.Empty,
            IssuedKeyId = string.Empty,
            ExpiresAt = expires,
            Status = LicenseActivationStatus.Active,
            ActivatedByUserId = userId,
        };
        db.Set<LicenseActivation>().Add(activation);

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        tenant?.Activate();

        AddEvent(LicenseEventType.Activated, "ok", null, license.Id, activation.Id, tenantId, null);
        await RecordAttemptAsync(license.KeyHash, true, ct, save: false);
        await db.SaveChangesAsync(ct);
    }

    // ---- shared steps
    private async Task<License> FindUsableLicenseAsync(string key, Guid? tenantId, CancellationToken ct)
    {
        var ip = requestInfo.IpAddress ?? "local";
        var now = clock.UtcNow;
        var since = now.AddMinutes(-LicensePolicy.AttemptWindowMinutes);

        var failures = await db.Set<LicenseAttempt>().CountAsync(a => a.IpAddress == ip && !a.IsSuccess && a.AttemptedAt > since, ct);
        if (failures >= LicensePolicy.MaxFailedAttempts)
        {
            AddEvent(LicenseEventType.KeyBruteForce, "blocked", "too_many_attempts", null, null, tenantId, null);
            await db.SaveChangesAsync(ct);
            throw new RequestRejectedException("too_many_attempts", $"Too many wrong keys. Please try again in {LicensePolicy.AttemptWindowMinutes} minutes.");
        }

        var normalized = LicenseKeyFormat.Normalize(key);
        if (!LicenseKeyFormat.IsWellFormed(normalized))
        {
            await RecordAttemptAsync(null, false, ct);
            throw InvalidKey();
        }

        var hash = HashKey(normalized);
        var license = await db.Set<License>().FirstOrDefaultAsync(l => l.KeyHash == hash, ct);
        if (license is null)
        {
            await RecordAttemptAsync(hash, false, ct);
            throw InvalidKey();
        }

        if (license.KeyPurpose == LicenseKeyPurpose.Upgrade)
        {
            throw new RequestRejectedException("key_not_supported", "Upgrade keys are applied by support. Please contact us.");
        }

        switch (license.Status)
        {
            case LicenseStatus.Revoked:
            case LicenseStatus.Suspended:
                throw new RequestRejectedException("key_revoked", "This license key cannot be used. Please contact support.");
            case LicenseStatus.Superseded:
                throw new RequestRejectedException("key_invalid", "This license key was replaced by another one.");
        }

        if (!tenantId.HasValue && license.TenantId.HasValue)
        {
            await RecordAttemptAsync(hash, false, ct);
            throw InvalidKey();
        }

        if (tenantId.HasValue && license.Status == LicenseStatus.Active && license.TenantId != tenantId)
        {
            await RecordAttemptAsync(hash, false, ct);
            throw InvalidKey();
        }

        if (tenantId.HasValue && license.Status == LicenseStatus.Active)
        {
            throw new RequestRejectedException("key_already_applied", "This key is already applied to your business.");
        }

        if (license.ExpiresAt is { } ends && now > ends.AddDays(LicensePolicy.GraceDays))
        {
            throw new RequestRejectedException("license_expired", "This license has already ended. Please buy a new key.");
        }

        return license;
    }

    private async Task<string> IssueAsync(License license, LicenseActivation activation, Plan plan, string fingerprint, string[] parts, CancellationToken ct)
    {
        if (!signer.IsAvailable)
        {
            throw new InvalidOperationException("This host has no license signing key.");
        }

        await EnsureSigningKeyRowAsync(ct);

        var now = clock.UtcNow;
        var lastSequence = await db.Set<LicenseActivation>()
            .Where(a => a.LicenseId == license.Id)
            .MaxAsync(a => (long?)a.IssuedSequence, ct) ?? 0;

        var sequence = Math.Max(UnixMs(now), lastSequence + 1);

        var payload = new LicensePayload(
            LicenseFileCodec.CurrentVersion, license.Id, activation.Id, sequence, license.KeyHint, license.KeyHash,
            license.LicenseType.ToString(), plan.Code, license.LicensedUserName ?? activation.ActivatedUserName,
            fingerprint, parts, now, activation.ActivatedAt, license.ExpiresAt, license.MaxDevices, license.MaxBranches,
            license.FeaturesJson ?? "{}", signer.KeyId);

        var (content, signatureHash) = LicenseFileCodec.Create(payload, signer);
        activation.IssuedSequence = sequence;
        activation.IssuedSignatureHash = signatureHash;
        activation.IssuedKeyId = signer.KeyId;
        return content;
    }

    private async Task EnsureSigningKeyRowAsync(CancellationToken ct)
    {
        var exists = await db.Set<LicenseSigningKey>().AnyAsync(k => k.KeyId == signer.KeyId, ct);
        if (!exists)
        {
            db.Set<LicenseSigningKey>().Add(new LicenseSigningKey
            {
                KeyId = signer.KeyId,
                Algorithm = LicenseSigningKeyAlgorithm.EcdsaP256,
                PublicKey = signer.PublicKey,
                PrivateKeyVaultRef = "config:License:SigningPrivateKey",
                ActiveFrom = clock.UtcNow,
                IsCurrent = true,
            });
        }
    }

    private async Task ConvertTrialAsync(string fingerprint, Guid licenseId, CancellationToken ct)
    {
        var trial = await db.Set<TrialRegistry>()
            .FirstOrDefaultAsync(t => t.ConvertedToLicenseId == null && t.MachineFingerprintHash == fingerprint, ct);
        if (trial is null)
        {
            return;
        }

        trial.ConvertedToLicenseId = licenseId;
        var trialLicense = await db.Set<License>().FirstOrDefaultAsync(l => l.Id == trial.LicenseId, ct);
        if (trialLicense is not null)
        {
            trialLicense.Status = LicenseStatus.Superseded;
        }
    }

    private async Task<Plan> DefaultPlanAsync(CancellationToken ct) =>
        await db.Set<Plan>().Where(p => p.IsActive).OrderBy(p => p.Code == DefaultPlanCode ? 0 : 1).ThenBy(p => p.SortOrder).FirstOrDefaultAsync(ct)
        ?? throw new RequestRejectedException("no_plan", "No plan exists on the License Server.");

    private async Task RecordAttemptAsync(string? keyHash, bool success, CancellationToken ct, bool save = true)
    {
        db.Set<LicenseAttempt>().Add(new LicenseAttempt
        {
            IpAddress = requestInfo.IpAddress ?? "local",
            KeyHashTried = keyHash,
            IsSuccess = success,
            AttemptedAt = clock.UtcNow,
        });

        if (!success)
        {
            AddEvent(LicenseEventType.ActivationFailed, "failed", "key_invalid", null, null, null, null);
        }

        if (save)
        {
            await db.SaveChangesAsync(ct);
        }
    }

    private void AddEvent(LicenseEventType type, string result, string? reason, Guid? licenseId, Guid? activationId, Guid? tenantId, string? fingerprint) =>
        db.Set<LicenseEvent>().Add(new LicenseEvent
        {
            LicenseId = licenseId,
            ActivationId = activationId,
            TenantId = tenantId,
            EventType = type,
            Result = result,
            ReasonCode = reason,
            IpAddress = requestInfo.IpAddress,
            FingerprintHash = fingerprint,
            OccurredAt = clock.UtcNow,
        });

    private static string[] ReadParts(string? json)
    {
        try
        {
            return string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<string[]>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static DeploymentMode ParseMode(string? mode) =>
        Enum.TryParse<DeploymentMode>(mode, true, out var parsed) ? parsed : DeploymentMode.Desktop;

    private static string? Trim(string? value, int max) =>
        value is null ? null : value.Length > max ? value[..max] : value;

    private static long UnixMs(DateTime utc) => new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
}
