using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockBatch: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockBatch : BranchEntity
{
    public Guid RawMaterialId { get; set; }
    public string? BatchNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public decimal QuantityRemaining { get; set; }
    public decimal UnitCost { get; set; }
    public Guid? GoodsReceiptItemId { get; set; }

    public RawMaterial RawMaterial { get; set; } = null!;
    public GoodsReceiptItem? GoodsReceiptItem { get; set; }
}
