using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseClockMark: tenant table. Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseClockMark : TenantEntity
{
    public DateTime SeenAtUtc { get; set; }
    public DateTime? LastBusinessRecordAt { get; set; }
    public DateTime? LastServerTimeUtc { get; set; }
    public long? UptimeSeconds { get; set; }
    public LicenseClockSource Source { get; set; }
    public string Hmac { get; set; } = string.Empty;
}
