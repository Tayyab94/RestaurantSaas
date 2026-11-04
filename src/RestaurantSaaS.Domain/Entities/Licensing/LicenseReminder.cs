using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseReminder: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseReminder : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid LicenseId { get; set; }
    public int MilestoneDays { get; set; }
    public NotificationChannel Channel { get; set; }
    public DateTime? SentAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public Tenant Tenant { get; set; } = null!;
    public License License { get; set; } = null!;
}
