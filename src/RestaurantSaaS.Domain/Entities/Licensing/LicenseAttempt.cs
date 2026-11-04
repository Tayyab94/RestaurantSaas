using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseAttempt: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseAttempt : BaseEntity
{
    public Guid? TenantId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? KeyHashTried { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime AttemptedAt { get; set; }
    public DateTime? LockedUntil { get; set; }

    public Tenant? Tenant { get; set; }
}
