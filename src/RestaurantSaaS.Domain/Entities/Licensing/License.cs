using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>License: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class License : BaseEntity
{
    public Guid? BatchId { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public string KeyHint { get; set; } = string.Empty;
    public string KeyChecksum { get; set; } = string.Empty;
    public LicenseType LicenseType { get; set; }
    public int? DurationMonths { get; set; }
    public int? DurationDays { get; set; }
    public Guid PlanId { get; set; }
    public int MaxActivations { get; set; }
    public int ActivationCount { get; set; }
    public int MaxDevices { get; set; }
    public int MaxBranches { get; set; }
    public string? FeaturesJson { get; set; }
    public LicenseStatus Status { get; set; }
    public string? LicensedUserName { get; set; }
    public string? LicensedPhone { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime? FirstActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedBy { get; set; }
    public string? RevokeReason { get; set; }
    public decimal? SoldPrice { get; set; }
    public string? Notes { get; set; }
    public Guid? ReplacesLicenseId { get; set; }
    public int Version { get; set; }
    public LicenseKeyPurpose KeyPurpose { get; set; }
    public LicenseType? UpgradeFromType { get; set; }

    public LicenseBatch? Batch { get; set; }
    public Plan Plan { get; set; } = null!;
    public Tenant? Tenant { get; set; }
    public License? ReplacesLicense { get; set; }
}
