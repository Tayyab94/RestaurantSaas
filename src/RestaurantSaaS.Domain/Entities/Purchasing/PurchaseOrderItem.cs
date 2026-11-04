using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PurchaseOrderItem: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class PurchaseOrderItem : BranchEntity
{
    public Guid PurchaseOrderId { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRatePercent { get; set; }
    public decimal LineTotal { get; set; }
    public Guid? SupplierItemId { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
    public SupplierItem? SupplierItem { get; set; }
}
