using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TrialRegistry: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class TrialRegistry : BaseEntity
{
    public string? MachineFingerprintHash { get; set; }
    public string? PhoneHash { get; set; }
    public string? EmailHash { get; set; }
    public Guid LicenseId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid? ConvertedToLicenseId { get; set; }

    public License License { get; set; } = null!;
    public License? ConvertedToLicense { get; set; }
}
