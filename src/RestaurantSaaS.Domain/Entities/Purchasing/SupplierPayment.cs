using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierPayment: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierPayment : BranchEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? GoodsReceiptId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public Guid PaidBy { get; set; }
    public Guid? ShiftId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public GoodsReceipt? GoodsReceipt { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public Shift? Shift { get; set; }
}
