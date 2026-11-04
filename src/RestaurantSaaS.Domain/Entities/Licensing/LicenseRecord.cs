using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseRecord: tenant table. Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseRecord : TenantEntity
{
    public long Sequence { get; set; }
    public Guid LicenseId { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public string KeyHint { get; set; } = string.Empty;
    public LicenseType LicenseType { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public string LicensedUserName { get; set; } = string.Empty;
    public string MachineFingerprintHash { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int MaxDevices { get; set; }
    public int MaxBranches { get; set; }
    public string FeaturesJson { get; set; } = string.Empty;
    public string SignatureHash { get; set; } = string.Empty;
    public string SignatureKeyId { get; set; } = string.Empty;
    public string SignedPayload { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public LicenseRecordSource Source { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public string PreviousRecordHash { get; set; } = string.Empty;
    public string RecordHash { get; set; } = string.Empty;
    public string RecordHmac { get; set; } = string.Empty;
}
