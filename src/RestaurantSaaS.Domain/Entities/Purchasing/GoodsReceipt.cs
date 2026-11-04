using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>GoodsReceipt: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class GoodsReceipt : BranchEntity
{
    public string GrnNumber { get; set; } = string.Empty;
    public Guid? PurchaseOrderId { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierInvoiceNo { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public DateOnly? ExpectedDate { get; set; }
    public bool IsLate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ReceivedBy { get; set; }

    public PurchaseOrder? PurchaseOrder { get; set; }
    public Supplier Supplier { get; set; } = null!;
}
