using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierAdvance: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierAdvance : BranchEntity
{
    public Guid SupplierId { get; set; }
    public decimal Amount { get; set; }
    public decimal UsedAmount { get; set; }
    public Guid PaymentMethodId { get; set; }
    public DateTime PaidAt { get; set; }
    public string? ReferenceNo { get; set; }
    public SupplierAdvanceStatus Status { get; set; }
    public Guid? SupplierPaymentId { get; set; }
    public string? Notes { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public SupplierPayment? SupplierPayment { get; set; }
}
