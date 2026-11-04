using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockAdjustmentItem: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockAdjustmentItem : BranchEntity
{
    public Guid StockAdjustmentId { get; set; }
    public Guid RawMaterialId { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal Difference { get; set; }
    public decimal UnitCost { get; set; }

    public StockAdjustment StockAdjustment { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
}
