using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseEvent: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseEvent : BaseEntity
{
    public Guid? LicenseId { get; set; }
    public Guid? ActivationId { get; set; }
    public Guid? TenantId { get; set; }
    public LicenseEventType EventType { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public string? IpAddress { get; set; }
    public string? FingerprintHash { get; set; }
    public string? AppVersion { get; set; }
    public string? DetailsJson { get; set; }
    public DateTime OccurredAt { get; set; }

    public License? License { get; set; }
    public LicenseActivation? Activation { get; set; }
    public Tenant? Tenant { get; set; }
}
