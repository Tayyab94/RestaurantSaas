using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseSigningKey: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseSigningKey : BaseEntity
{
    public string KeyId { get; set; } = string.Empty;
    public LicenseSigningKeyAlgorithm Algorithm { get; set; }
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKeyVaultRef { get; set; } = string.Empty;
    public DateTime ActiveFrom { get; set; }
    public DateTime? RetiredAt { get; set; }
    public bool IsCurrent { get; set; }
}
