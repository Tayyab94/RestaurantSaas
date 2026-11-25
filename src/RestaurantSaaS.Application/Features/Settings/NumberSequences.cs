using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Settings;

// FR-M05-05: document numbers follow a prefix, padding and reset rule per branch.

public sealed record DocumentKind(string Type, string Label, string Prefix, int Padding, NumberSequenceResetPolicy Reset);

public static class DocumentKinds
{
    public const string Order = "Order";
    public const string Invoice = "Invoice";
    public const string Payment = "Payment";
    public const string Ticket = "Ticket";
    public const string Shift = "Shift";

    /// <summary>The documents that are numbered, with the starting rule of a new branch.</summary>
    public static IReadOnlyList<DocumentKind> All { get; } =
    [
        new(Order, "Order", "ORD-", 5, NumberSequenceResetPolicy.Daily),
        new(Invoice, "Invoice", "INV-", 6, NumberSequenceResetPolicy.Yearly),
        new(Payment, "Payment", "PAY-", 6, NumberSequenceResetPolicy.Yearly),
        new(Ticket, "Kitchen ticket", "KOT-", 4, NumberSequenceResetPolicy.Daily),
        new(Shift, "Shift", "SH-", 4, NumberSequenceResetPolicy.Never),
    ];

    public static string Format(string prefix, long value, int padding, string? suffix) =>
        prefix + value.ToString(CultureInfo.InvariantCulture).PadLeft(Math.Max(1, padding), '0') + suffix;
}

/// <summary>
/// Hands out the next document number of a branch (M09 orders and tickets, M11 invoices and payments, M15 shifts).
/// It changes the row but does not save: the caller saves it in the same SaveChanges as the new document, so a failed
/// document does not burn a number. Two calls at the same moment hit the row's concurrency token; the second must retry.
/// </summary>
internal sealed class NumberSequenceService(IApplicationDbContext db, ICurrentUser current, IClock clock)
{
    public async Task<string> NextAsync(Guid branchId, string documentType, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Numbers belong to a business.");

        var kind = DocumentKinds.All.FirstOrDefault(k => k.Type == documentType)
            ?? throw new RequestRejectedException("unknown_document", "This document is not numbered.");

        var timeZoneId = await db.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => t.TimeZone)
            .FirstOrDefaultAsync(ct);

        var today = DateOnly.FromDateTime(ToLocal(clock.UtcNow, timeZoneId));

        var row = await db.Set<NumberSequence>().FirstOrDefaultAsync(n => n.BranchId == branchId && n.DocumentType == documentType, ct);

        if (row is null)
        {
            row = new NumberSequence
            {
                TenantId = tenantId,
                BranchId = branchId,
                DocumentType = documentType,
                Prefix = kind.Prefix,
                Padding = kind.Padding,
                ResetPolicy = kind.Reset,
                CurrentValue = 0,
                LastResetOn = today,
            };
            db.Set<NumberSequence>().Add(row);
        }

        if (NeedsReset(row, today))
        {
            row.CurrentValue = 0;
            row.LastResetOn = today;
        }

        row.CurrentValue++;
        return DocumentKinds.Format(row.Prefix, row.CurrentValue, row.Padding, row.Suffix);
    }

    private static bool NeedsReset(NumberSequence row, DateOnly today) =>
        row.LastResetOn is null
        || row.ResetPolicy switch
        {
            NumberSequenceResetPolicy.Daily => row.LastResetOn != today,
            NumberSequenceResetPolicy.Monthly => row.LastResetOn.Value.Year != today.Year || row.LastResetOn.Value.Month != today.Month,
            NumberSequenceResetPolicy.Yearly => row.LastResetOn.Value.Year != today.Year,
            _ => false,
        };

    private static DateTime ToLocal(DateTime utc, string? timeZoneId)
    {
        var value = DateTime.SpecifyKind(utc, DateTimeKind.Utc);

        try
        {
            return string.IsNullOrWhiteSpace(timeZoneId)
                ? value
                : TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }
        catch (TimeZoneNotFoundException)
        {
            return value;
        }
        catch (InvalidTimeZoneException)
        {
            return value;
        }
    }
}

[RequirePermission(Permissions.SettingsView)]
public sealed record GetNumberSequencesQuery(Guid BranchId) : IRequest<IReadOnlyList<NumberSequenceDto>>;

internal sealed class GetNumberSequencesHandler(IApplicationDbContext db)
    : IRequestHandler<GetNumberSequencesQuery, IReadOnlyList<NumberSequenceDto>>
{
    public async Task<IReadOnlyList<NumberSequenceDto>> HandleAsync(GetNumberSequencesQuery request, CancellationToken ct)
    {
        if (!await db.Branches.AnyAsync(b => b.Id == request.BranchId, ct))
        {
            throw new RequestRejectedException("branch_not_found", "The branch does not exist.");
        }

        var rows = await db.Set<NumberSequence>().AsNoTracking()
            .Where(n => n.BranchId == request.BranchId)
            .ToListAsync(ct);

        // A document with no saved row shows its starting rule.
        return DocumentKinds.All
            .Select(kind =>
            {
                var row = rows.FirstOrDefault(r => r.DocumentType == kind.Type);
                var prefix = row?.Prefix ?? kind.Prefix;
                var padding = row?.Padding ?? kind.Padding;
                var current = row?.CurrentValue ?? 0;

                return new NumberSequenceDto(
                    kind.Type,
                    kind.Label,
                    prefix,
                    row?.Suffix,
                    padding,
                    (row?.ResetPolicy ?? kind.Reset).ToString(),
                    current,
                    DocumentKinds.Format(prefix, current + 1, padding, row?.Suffix));
            })
            .ToList();
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SaveNumberSequencesCommand(Guid BranchId, IReadOnlyList<NumberSequenceInputDto> Items) : IRequest<bool>;

internal sealed partial class SaveNumberSequencesValidator : AbstractValidator<SaveNumberSequencesCommand>
{
    public SaveNumberSequencesValidator()
    {
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DocumentType)
                .Must(type => DocumentKinds.All.Any(k => k.Type == type))
                .WithMessage("Unknown document.");

            item.RuleFor(i => i.Prefix).MaximumLength(20).Matches(TextPattern()).WithMessage("The prefix may only have letters, digits and - _ / .");
            item.RuleFor(i => i.Suffix).MaximumLength(20).Matches(TextPattern()).When(i => !string.IsNullOrEmpty(i.Suffix))
                .WithMessage("The suffix may only have letters, digits and - _ / .");
            item.RuleFor(i => i.Padding).InclusiveBetween(1, 12);
            item.RuleFor(i => i.ResetPolicy)
                .Must(value => Enum.TryParse<NumberSequenceResetPolicy>(value, out _))
                .WithMessage("Choose a reset rule.");
        });

        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.DocumentType).Distinct().Count() == items.Count)
            .WithMessage("A document is listed twice.");
    }

    [GeneratedRegex(@"^[A-Za-z0-9_\-/.]*$")]
    private static partial Regex TextPattern();
}

internal sealed class SaveNumberSequencesHandler(IApplicationDbContext db, ICurrentUser current, IClock clock, AuditTrail audit)
    : IRequestHandler<SaveNumberSequencesCommand, bool>
{
    public async Task<bool> HandleAsync(SaveNumberSequencesCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Numbers belong to a business.");

        if (!await db.Branches.AnyAsync(b => b.Id == request.BranchId, ct))
        {
            throw new RequestRejectedException("branch_not_found", "The branch does not exist.");
        }

        var rows = await db.Set<NumberSequence>().Where(n => n.BranchId == request.BranchId).ToListAsync(ct);
        var today = DateOnly.FromDateTime(clock.UtcNow);

        foreach (var item in request.Items)
        {
            var row = rows.FirstOrDefault(r => r.DocumentType == item.DocumentType);

            if (row is null)
            {
                row = new NumberSequence
                {
                    TenantId = tenantId,
                    BranchId = request.BranchId,
                    DocumentType = item.DocumentType,
                    CurrentValue = 0,
                    LastResetOn = today,
                };
                db.Set<NumberSequence>().Add(row);
            }

            row.Prefix = item.Prefix.Trim();
            row.Suffix = string.IsNullOrWhiteSpace(item.Suffix) ? null : item.Suffix.Trim();
            row.Padding = item.Padding;
            row.ResetPolicy = Enum.Parse<NumberSequenceResetPolicy>(item.ResetPolicy);
        }

        audit.Add(
            "number_sequences.updated",
            "Branch",
            request.BranchId,
            newValues: request.Items.Select(i => new { i.DocumentType, i.Prefix, i.Suffix, i.Padding, i.ResetPolicy }).ToList(),
            branchId: request.BranchId);

        await db.SaveChangesAsync(ct);
        return true;
    }
}
