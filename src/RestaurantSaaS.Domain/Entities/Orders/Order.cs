using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Order: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class Order : BranchEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public DateOnly BusinessDate { get; set; }
    public OrderOrderType OrderType { get; set; }
    public OrderSource Source { get; set; }
    public Guid? ExternalOrderId { get; set; }
    public OrderStatus Status { get; set; }
    public Guid? TableId { get; set; }
    public int? GuestCount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? QrSessionId { get; set; }
    public Guid WaiterId { get; set; }
    public Guid? CashierId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ShiftId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal DeliveryCharge { get; set; }
    public decimal TipAmount { get; set; }
    public decimal RoundOff { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public OrderPaymentStatus PaymentStatus { get; set; }
    public string? Notes { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReasonNote { get; set; }
    public Guid? CancelReasonId { get; set; }
    public Guid? CancelledBy { get; set; }
    public Guid? ParentOrderId { get; set; }
    public bool IsComplimentary { get; set; }
    public int? TokenNumber { get; set; }
    public Guid? PriceListId { get; set; }
    public OrderPriority Priority { get; set; }
    public string? PriorityReason { get; set; }
    public DateTime? AllItemsReadyAt { get; set; }

    public RestaurantTable? Table { get; set; }
    public QrOrderSession? QrSession { get; set; }
    public User Waiter { get; set; } = null!;
    public User? Cashier { get; set; }
    public Customer? Customer { get; set; }
    public Shift? Shift { get; set; }
    public ReasonCode? CancelReason { get; set; }
    public Order? ParentOrder { get; set; }
    public PriceList? PriceList { get; set; }
}
