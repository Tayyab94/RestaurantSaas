using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>GoodsReceiptItem: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class GoodsReceiptItem : BranchEntity
{
    public Guid GoodsReceiptId { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public string? RejectionReason { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRatePercent { get; set; }
    public decimal LineTotal { get; set; }
    public string? BatchNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public Guid? SupplierItemId { get; set; }

    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
    public SupplierItem? SupplierItem { get; set; }
}
