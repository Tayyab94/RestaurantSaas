using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseCheckLog: tenant table. Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseCheckLog : TenantEntity
{
    public DateTime CheckedAt { get; set; }
    public LicenseCheckTrigger Trigger { get; set; }
    public LicenseCheckResult Result { get; set; }
    public string? ReasonCode { get; set; }
    public string? FileHash { get; set; }
    public long? RecordSequence { get; set; }
    public string? AppVersion { get; set; }
}
