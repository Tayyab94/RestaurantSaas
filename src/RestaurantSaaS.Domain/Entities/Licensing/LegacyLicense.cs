using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LegacyLicense: tenant table. Part of the Licensing module. See Database Schema.</summary>
public sealed class LegacyLicense : TenantEntity
{
    public string UserName { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public Guid HardwareId { get; set; }
    public int LicenseType { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime ImportedAt { get; set; }
}
