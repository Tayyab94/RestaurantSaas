using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseActivation: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseActivation : BaseEntity
{
    public Guid LicenseId { get; set; }
    public Guid? TenantId { get; set; }
    public string? MachineFingerprintHash { get; set; }
    public string? FingerprintPartsHashJson { get; set; }
    public string? MachineName { get; set; }
    public string? OsVersion { get; set; }
    public string? AppVersion { get; set; }
    public DeploymentMode DeploymentMode { get; set; }
    public string ActivatedUserName { get; set; } = string.Empty;
    public LicenseActivationActivationMethod ActivationMethod { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public string? LastIpAddress { get; set; }
    public long IssuedSequence { get; set; }
    public string IssuedSignatureHash { get; set; } = string.Empty;
    public string IssuedKeyId { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public LicenseActivationStatus Status { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public Guid? DeactivatedBy { get; set; }
    public string? DeactivationReason { get; set; }
    public Guid? ActivatedByUserId { get; set; }
    public string? UserAgent { get; set; }
    public bool PendingDelivery { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public License License { get; set; } = null!;
    public Tenant? Tenant { get; set; }
    public User? ActivatedByUser { get; set; }
}
