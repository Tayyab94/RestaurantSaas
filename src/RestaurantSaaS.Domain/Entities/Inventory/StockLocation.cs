using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockLocation: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockLocation : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public StockLocationLocationType LocationType { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
