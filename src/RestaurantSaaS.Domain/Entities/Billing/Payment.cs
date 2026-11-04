using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Payment: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class Payment : BranchEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public PaymentPaymentType PaymentType { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public decimal? TenderedAmount { get; set; }
    public decimal? ChangeAmount { get; set; }
    public decimal TipAmount { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid? GatewayTransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PaidAt { get; set; }
    public Guid ReceivedBy { get; set; }
    public Guid? ShiftId { get; set; }

    public Invoice? Invoice { get; set; }
    public Order? Order { get; set; }
    public Customer? Customer { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public Shift? Shift { get; set; }
}
