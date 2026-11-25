using System.Globalization;
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

// FR-M05-02: tax rates, service charge, currency and rounding.

[RequirePermission(Permissions.SettingsView)]
public sealed record GetCurrenciesQuery : IRequest<IReadOnlyList<CurrencyDto>>;

internal sealed class GetCurrenciesHandler : IRequestHandler<GetCurrenciesQuery, IReadOnlyList<CurrencyDto>>
{
    public Task<IReadOnlyList<CurrencyDto>> HandleAsync(GetCurrenciesQuery request, CancellationToken ct) =>
        Task.FromResult(CurrencyCatalog.All);
}

[RequirePermission(Permissions.SettingsView)]
public sealed record GetTaxRatesQuery : IRequest<IReadOnlyList<TaxRateDto>>;

internal sealed class GetTaxRatesHandler(IApplicationDbContext db) : IRequestHandler<GetTaxRatesQuery, IReadOnlyList<TaxRateDto>>
{
    public async Task<IReadOnlyList<TaxRateDto>> HandleAsync(GetTaxRatesQuery request, CancellationToken ct)
    {
        var rows = await db.Set<TaxRate>().AsNoTracking().OrderBy(t => t.Name).ToListAsync(ct);

        return rows
            .Select(t => new TaxRateDto(
                t.Id, t.Name, t.Rate, t.TaxType.ToString(), t.AppliesTo.ToString(), t.IsInclusive, t.IsDefault, t.FbrTaxCode))
            .ToList();
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SaveTaxRateCommand(
    Guid? Id,
    string Name,
    decimal Rate,
    string TaxType,
    string AppliesTo,
    bool IsInclusive,
    bool IsDefault,
    string? FbrTaxCode) : IRequest<Guid>;

internal sealed class SaveTaxRateValidator : AbstractValidator<SaveTaxRateCommand>
{
    public SaveTaxRateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.TaxType)
            .Must(value => Enum.TryParse<TaxRateTaxType>(value, out _))
            .WithMessage("Choose Percent or Fixed.");

        RuleFor(x => x.AppliesTo)
            .Must(value => Enum.TryParse<TaxRateAppliesTo>(value, out _))
            .WithMessage("Choose what the tax applies to.");

        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0m).LessThanOrEqualTo(1_000_000m);

        RuleFor(x => x.Rate).LessThanOrEqualTo(100m)
            .When(x => x.TaxType == nameof(TaxRateTaxType.Percent))
            .WithMessage("A percent rate cannot be more than 100.");

        RuleFor(x => x.FbrTaxCode).MaximumLength(100);
    }
}

internal sealed class SaveTaxRateHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SaveTaxRateCommand, Guid>
{
    public async Task<Guid> HandleAsync(SaveTaxRateCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Tax rates belong to a business.");

        TaxRate rate;
        var isNew = request.Id is null;

        if (isNew)
        {
            rate = new TaxRate { TenantId = tenantId };
            db.Set<TaxRate>().Add(rate);
        }
        else
        {
            rate = await db.Set<TaxRate>().FirstOrDefaultAsync(t => t.Id == request.Id, ct)
                ?? throw new RequestRejectedException("tax_not_found", "The tax rate does not exist.");
        }

        rate.Name = request.Name.Trim();
        rate.Rate = request.Rate;
        rate.TaxType = Enum.Parse<TaxRateTaxType>(request.TaxType);
        rate.AppliesTo = Enum.Parse<TaxRateAppliesTo>(request.AppliesTo);
        rate.IsInclusive = request.IsInclusive;
        rate.IsDefault = request.IsDefault;
        rate.FbrTaxCode = string.IsNullOrWhiteSpace(request.FbrTaxCode) ? null : request.FbrTaxCode.Trim();

        audit.Add(
            isNew ? "tax.created" : "tax.updated",
            "TaxRate",
            rate.Id,
            newValues: new { rate.Name, rate.Rate, request.TaxType, request.AppliesTo, rate.IsInclusive, rate.IsDefault });

        await db.SaveChangesAsync(ct);
        return rate.Id;
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record DeleteTaxRateCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteTaxRateHandler(IApplicationDbContext db, AuditTrail audit) : IRequestHandler<DeleteTaxRateCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteTaxRateCommand request, CancellationToken ct)
    {
        var rate = await db.Set<TaxRate>().FirstOrDefaultAsync(t => t.Id == request.Id, ct)
            ?? throw new RequestRejectedException("tax_not_found", "The tax rate does not exist.");

        // The data layer turns Remove into a soft delete.
        db.Set<TaxRate>().Remove(rate);
        audit.Add("tax.deleted", "TaxRate", rate.Id, oldValues: new { rate.Name, rate.Rate });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

[RequirePermission(Permissions.SettingsView)]
public sealed record GetBillingSettingsQuery : IRequest<BillingSettingsDto>;

internal sealed class GetBillingSettingsHandler(IApplicationDbContext db, ICurrentUser current, SettingStore settings)
    : IRequestHandler<GetBillingSettingsQuery, BillingSettingsDto>
{
    public async Task<BillingSettingsDto> HandleAsync(GetBillingSettingsQuery request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Billing settings belong to a business.");

        var currency = await db.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => t.CurrencyCode)
            .FirstOrDefaultAsync(ct) ?? "PKR";

        var billing = await settings.GetBillingAsync(ct);

        return new BillingSettingsDto(
            currency,
            billing.ServiceChargeEnabled,
            billing.ServiceChargePercent,
            billing.RoundingMode.ToString(),
            billing.RoundingStep);
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SaveBillingSettingsCommand(
    string CurrencyCode,
    bool ServiceChargeEnabled,
    decimal ServiceChargePercent,
    string RoundingMode,
    int RoundingStep) : IRequest<bool>;

internal sealed class SaveBillingSettingsValidator : AbstractValidator<SaveBillingSettingsCommand>
{
    public SaveBillingSettingsValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .Must(code => CurrencyCatalog.Find(code) is not null)
            .WithMessage("Choose a currency from the list.");

        RuleFor(x => x.ServiceChargePercent).InclusiveBetween(0m, 100m);

        RuleFor(x => x.RoundingMode)
            .Must(value => Enum.TryParse<BillRoundingMode>(value, out _))
            .WithMessage("Choose a rounding rule.");

        RuleFor(x => x.RoundingStep)
            .Must(step => step is 1 or 5 or 10)
            .WithMessage("Round to 1, 5 or 10.");
    }
}

internal sealed class SaveBillingSettingsHandler(IApplicationDbContext db, ICurrentUser current, SettingStore settings, AuditTrail audit)
    : IRequestHandler<SaveBillingSettingsCommand, bool>
{
    public async Task<bool> HandleAsync(SaveBillingSettingsCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Billing settings belong to a business.");

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new RequestRejectedException("not_found", "The business was not found.");

        tenant.SetCurrency(request.CurrencyCode);

        var percent = request.ServiceChargePercent.ToString(CultureInfo.InvariantCulture);

        await settings.SetAsync(tenantId, null, SettingKeys.BillingGroup, SettingKeys.ServiceChargeEnabled, request.ServiceChargeEnabled ? "true" : "false", "bool", ct);
        await settings.SetAsync(tenantId, null, SettingKeys.BillingGroup, SettingKeys.ServiceChargePercent, percent, "decimal", ct);
        await settings.SetAsync(tenantId, null, SettingKeys.BillingGroup, SettingKeys.RoundingMode, request.RoundingMode, "string", ct);
        await settings.SetAsync(tenantId, null, SettingKeys.BillingGroup, SettingKeys.RoundingStep, request.RoundingStep.ToString(CultureInfo.InvariantCulture), "int", ct);

        audit.Add(
            "billing.settings.updated",
            "Tenant",
            tenantId,
            newValues: new { request.CurrencyCode, request.ServiceChargeEnabled, request.ServiceChargePercent, request.RoundingMode, request.RoundingStep });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Works out a sample bill from the saved settings (the "done when" check of M05).</summary>
[RequirePermission(Permissions.SettingsView)]
public sealed record PreviewTestBillQuery(decimal Subtotal) : IRequest<TestBillDto>;

internal sealed class PreviewTestBillValidator : AbstractValidator<PreviewTestBillQuery>
{
    public PreviewTestBillValidator() => RuleFor(x => x.Subtotal).GreaterThan(0m).LessThanOrEqualTo(100_000_000m);
}

internal sealed class PreviewTestBillHandler(IApplicationDbContext db, ICurrentUser current, SettingStore settings)
    : IRequestHandler<PreviewTestBillQuery, TestBillDto>
{
    public async Task<TestBillDto> HandleAsync(PreviewTestBillQuery request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Billing settings belong to a business.");

        var currencyCode = await db.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => t.CurrencyCode)
            .FirstOrDefaultAsync(ct) ?? "PKR";

        var places = CurrencyCatalog.Find(currencyCode)?.DecimalPlaces ?? 2;
        var billing = await settings.GetBillingAsync(ct);

        // Every tax marked "default" is added to a new bill.
        var rules = (await db.Set<TaxRate>().AsNoTracking().Where(t => t.IsDefault).OrderBy(t => t.Name).ToListAsync(ct))
            .Select(t => new BillTaxRule(t.Name, t.Rate, t.TaxType == TaxRateTaxType.Fixed, t.IsInclusive))
            .ToList();

        var result = BillCalculator.Calculate(
            request.Subtotal,
            billing.ServiceChargeEnabled ? billing.ServiceChargePercent : 0m,
            rules,
            new BillRounding(billing.RoundingMode, billing.RoundingStep),
            places);

        var lines = new List<TestBillLineDto> { new("Items total", result.Subtotal, false) };

        if (billing.ServiceChargeEnabled)
        {
            lines.Add(new TestBillLineDto($"Service charge ({billing.ServiceChargePercent:0.##}%)", result.ServiceCharge, false));
        }

        foreach (var tax in result.Taxes)
        {
            var label = tax.Rate > 0 && rules.Any(r => r.Name == tax.Name && !r.IsFixed)
                ? $"{tax.Name} ({tax.Rate:0.##}%)"
                : tax.Name;

            lines.Add(new TestBillLineDto(tax.IsInclusive ? $"{label}, included in prices" : label, tax.Amount, false));
        }

        if (result.Rounding != 0m)
        {
            lines.Add(new TestBillLineDto("Rounding", result.Rounding, false));
        }

        lines.Add(new TestBillLineDto("Total to pay", result.Total, true));

        return new TestBillDto(lines, result.Total, currencyCode);
    }
}
