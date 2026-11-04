using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LicenseBatch: platform table (no tenant). Part of the Licensing module. See Database Schema.</summary>
public sealed class LicenseBatch : BaseEntity
{
    public string BatchNumber { get; set; } = string.Empty;
    public LicenseType LicenseType { get; set; }
    public Guid PlanId { get; set; }
    public int Quantity { get; set; }
    public int MaxActivations { get; set; }
    public int MaxDevices { get; set; }
    public int MaxBranches { get; set; }
    public Guid GeneratedBy { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string? Notes { get; set; }

    public Plan Plan { get; set; } = null!;
}
