using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>BranchStock: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class BranchStock : BranchEntity
{
    public Guid RawMaterialId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal AverageCost { get; set; }
    public decimal? ReorderLevel { get; set; }
    public decimal? ParLevel { get; set; }
    public decimal? MaxLevel { get; set; }
    public DateTime? LastCountedAt { get; set; }

    public RawMaterial RawMaterial { get; set; } = null!;
    public StockLocation? Location { get; set; }
}
