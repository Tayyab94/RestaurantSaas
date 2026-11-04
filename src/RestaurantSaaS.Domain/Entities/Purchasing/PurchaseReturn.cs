using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PurchaseReturn: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class PurchaseReturn : BranchEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid? GoodsReceiptId { get; set; }
    public DateOnly ReturnDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ApprovedBy { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public GoodsReceipt? GoodsReceipt { get; set; }
}
