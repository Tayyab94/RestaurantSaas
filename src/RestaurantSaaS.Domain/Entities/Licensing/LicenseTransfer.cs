using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseTransfer: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseTransfer : BaseEntity
{
    public Guid LicenseId { get; set; }
    public Guid FromActivationId { get; set; }
    public string? ToFingerprintHash { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public LicenseTransferStatus Status { get; set; }
    public int TransferCountAfter { get; set; }

    public License License { get; set; } = null!;
    public LicenseActivation FromActivation { get; set; } = null!;
}
