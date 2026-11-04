using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseChange: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseChange : BaseEntity
{
    public Guid LicenseId { get; set; }
    public Guid? NewLicenseId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ActivationId { get; set; }
    public LicenseChangeType ChangeType { get; set; }
    public LicenseType? FromType { get; set; }
    public LicenseType? ToType { get; set; }
    public Guid? FromPlanId { get; set; }
    public Guid? ToPlanId { get; set; }
    public DateTime? FromExpiresAt { get; set; }
    public DateTime? ToExpiresAt { get; set; }
    public string? FromLimitsJson { get; set; }
    public string? ToLimitsJson { get; set; }
    public LicenseChangeEffectiveMode EffectiveMode { get; set; }
    public DateTime EffectiveAt { get; set; }
    public decimal? PriceDifference { get; set; }
    public decimal? CreditAmount { get; set; }
    public Guid? UpgradeKeyLicenseId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public LicenseChangeRequestedByRole RequestedByRole { get; set; }
    public Guid? ApprovedBy { get; set; }
    public LicenseChangeStatus Status { get; set; }
    public DateTime? AppliedAt { get; set; }
    public long? ResultingSequence { get; set; }
    public string? Notes { get; set; }

    public License License { get; set; } = null!;
    public License? NewLicense { get; set; }
    public Tenant? Tenant { get; set; }
    public LicenseActivation? Activation { get; set; }
    public Plan? FromPlan { get; set; }
    public Plan? ToPlan { get; set; }
    public License? UpgradeKeyLicense { get; set; }
}
