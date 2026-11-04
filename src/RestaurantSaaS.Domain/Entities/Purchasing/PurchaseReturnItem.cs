using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PurchaseReturnItem: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class PurchaseReturnItem : BranchEntity
{
    public Guid PurchaseReturnId { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public Guid? BatchId { get; set; }

    public PurchaseReturn PurchaseReturn { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
    public StockBatch? Batch { get; set; }
}
