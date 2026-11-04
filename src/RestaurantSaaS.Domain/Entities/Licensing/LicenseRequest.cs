using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseRequest: tenant table. Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseRequest : TenantEntity
{
    public LicenseRequestType RequestType { get; set; }
    public string KeyHint { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FingerprintHash { get; set; } = string.Empty;
    public string? RequestFilePath { get; set; }
    public string? ResponseFilePath { get; set; }
    public LicenseRequestStatus Status { get; set; }
    public DateTime? ImportedAt { get; set; }
}
