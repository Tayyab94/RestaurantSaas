using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LoyaltyProgram: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class LoyaltyProgram : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal PointsPerAmount { get; set; }
    public decimal AmountUnit { get; set; }
    public decimal RedeemValuePerPoint { get; set; }
    public int MinRedeemPoints { get; set; }
    public int? ExpiryMonths { get; set; }
    public string? TiersJson { get; set; }
    public bool IsActive { get; set; }
}
