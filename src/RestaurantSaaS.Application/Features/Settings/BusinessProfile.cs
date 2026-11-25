using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Features.Onboarding;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Settings;

// FR-M05-01: business profile and logo.

[RequirePermission(Permissions.SettingsView)]
public sealed record GetBusinessProfileQuery : IRequest<BusinessProfileDto>;

internal sealed class GetBusinessProfileHandler(IApplicationDbContext db, ICurrentUser current, SettingStore settings)
    : IRequestHandler<GetBusinessProfileQuery, BusinessProfileDto>
{
    public async Task<BusinessProfileDto> HandleAsync(GetBusinessProfileQuery request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Business settings belong to a business.");

        var tenant = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new RequestRejectedException("not_found", "The business was not found.");

        var values = await settings.GetAsync(
            null,
            [SettingKeys.BusinessAddress, SettingKeys.BusinessTaxNumber, SettingKeys.BusinessLogo],
            ct);

        return new BusinessProfileDto(
            tenant.Code,
            tenant.Name,
            tenant.OwnerName,
            tenant.Email,
            tenant.Phone,
            tenant.BusinessType.ToString(),
            values.GetValueOrDefault(SettingKeys.BusinessAddress),
            values.GetValueOrDefault(SettingKeys.BusinessTaxNumber),
            tenant.TimeZone,
            tenant.DefaultLanguage,
            values.GetValueOrDefault(SettingKeys.BusinessLogo));
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SaveBusinessProfileCommand(
    string Name,
    string OwnerName,
    string? Email,
    string? Phone,
    string BusinessType,
    string? Address,
    string? TaxNumber,
    string TimeZone,
    string DefaultLanguage) : IRequest<bool>;

internal sealed class SaveBusinessProfileValidator : AbstractValidator<SaveBusinessProfileCommand>
{
    public SaveBusinessProfileValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OwnerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone).NotEmpty().WithMessage("A phone number is required.")
            .Must(p => PhoneNumber.TryNormalize(p, out _)).WithMessage("Enter a valid phone number, for example 0300 1234567.");

        RuleFor(x => x.BusinessType)
            .Must(value => Enum.TryParse<BusinessType>(value, out _))
            .WithMessage("Choose a business type.");

        RuleFor(x => x.Address).MaximumLength(400);
        RuleFor(x => x.TaxNumber).MaximumLength(50);

        RuleFor(x => x.TimeZone)
            .Must(IsKnownTimeZone)
            .WithMessage("Choose a valid time zone.");

        RuleFor(x => x.DefaultLanguage)
            .Must(value => value is "en" or "ur")
            .WithMessage("Choose English or Urdu.");
    }

    private static bool IsKnownTimeZone(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(id);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}

internal sealed class SaveBusinessProfileHandler(
    IApplicationDbContext db,
    ICurrentUser current,
    IClock clock,
    SettingStore settings,
    AuditTrail audit) : IRequestHandler<SaveBusinessProfileCommand, bool>
{
    public async Task<bool> HandleAsync(SaveBusinessProfileCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Business settings belong to a business.");

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new RequestRejectedException("not_found", "The business was not found.");

        PhoneNumber.TryNormalize(request.Phone, out var phone);
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLowerInvariant();

        // Phone and email are unique across all businesses, so the check ignores the tenant filter.
        if (await db.Tenants.IgnoreQueryFilters().AnyAsync(t => !t.IsDeleted && t.Id != tenantId && t.Phone == phone, ct))
        {
            throw new RequestRejectedException("phone_exists", "This phone number is already used by another business.");
        }

        if (email is not null
            && await db.Tenants.IgnoreQueryFilters().AnyAsync(t => !t.IsDeleted && t.Id != tenantId && t.Email == email, ct))
        {
            throw new RequestRejectedException("email_exists", "This email is already used by another business.");
        }

        tenant.UpdateProfile(
            request.Name,
            request.OwnerName,
            email,
            phone,
            Enum.Parse<BusinessType>(request.BusinessType),
            request.TimeZone,
            request.DefaultLanguage);

        var address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        var taxNumber = string.IsNullOrWhiteSpace(request.TaxNumber) ? null : request.TaxNumber.Trim();

        await settings.SetAsync(tenantId, null, SettingKeys.BusinessGroup, SettingKeys.BusinessAddress, address, "string", ct);
        await settings.SetAsync(tenantId, null, SettingKeys.BusinessGroup, SettingKeys.BusinessTaxNumber, taxNumber, "string", ct);

        // FR-M05-07: the step ticks itself once the business has a name, a phone and an address.
        if (address is not null)
        {
            await OnboardingTick.DoneAsync(db, tenantId, current.UserId, clock.UtcNow, OnboardingSteps.BusinessInfoKey, ct);
        }

        audit.Add(
            "business.profile.updated",
            "Tenant",
            tenant.Id,
            newValues: new { tenant.Name, tenant.OwnerName, request.BusinessType, request.TimeZone, request.DefaultLanguage });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SetBusinessLogoCommand(string? DataUri) : IRequest<bool>;

/// <summary>A logo is a small PNG or JPEG kept as a data URI in the settings (about 200 KB at most).</summary>
internal static class LogoRules
{
    public const int MaxLength = 280_000;
    private const string PngPrefix = "data:image/png;base64,";
    private const string JpegPrefix = "data:image/jpeg;base64,";

    public static bool IsValid(string? dataUri)
    {
        if (string.IsNullOrEmpty(dataUri) || dataUri.Length > MaxLength)
        {
            return false;
        }

        var isPng = dataUri.StartsWith(PngPrefix, StringComparison.Ordinal);

        if (!isPng && !dataUri.StartsWith(JpegPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        var base64 = dataUri[(dataUri.IndexOf(',') + 1)..];
        var buffer = new byte[base64.Length];

        if (!Convert.TryFromBase64String(base64, buffer, out var written) || written < 4)
        {
            return false;
        }

        // The first bytes must match the declared type.
        return isPng
            ? buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47
            : buffer[0] == 0xFF && buffer[1] == 0xD8;
    }
}

internal sealed class SetBusinessLogoValidator : AbstractValidator<SetBusinessLogoCommand>
{
    public SetBusinessLogoValidator() =>
        RuleFor(x => x.DataUri)
            .Must(LogoRules.IsValid)
            .WithMessage("Use a PNG or JPEG picture of up to 200 KB.")
            .When(x => !string.IsNullOrEmpty(x.DataUri));
}

internal sealed class SetBusinessLogoHandler(IApplicationDbContext db, ICurrentUser current, SettingStore settings, AuditTrail audit)
    : IRequestHandler<SetBusinessLogoCommand, bool>
{
    public async Task<bool> HandleAsync(SetBusinessLogoCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Business settings belong to a business.");

        await settings.SetAsync(tenantId, null, SettingKeys.BusinessGroup, SettingKeys.BusinessLogo, request.DataUri, "string", ct);

        // The picture itself is never written to the audit log.
        audit.Add(string.IsNullOrEmpty(request.DataUri) ? "business.logo.removed" : "business.logo.changed", "Tenant", tenantId);

        await db.SaveChangesAsync(ct);
        return true;
    }
}
