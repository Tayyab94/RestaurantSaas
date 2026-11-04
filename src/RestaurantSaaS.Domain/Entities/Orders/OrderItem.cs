using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderItem: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderItem : BranchEntity
{
    public Guid OrderId { get; set; }
    public Guid? MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public Guid? DealId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid? TaxRateId { get; set; }
    public decimal TaxRatePercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public string? Notes { get; set; }
    public int? CourseNumber { get; set; }
    public int? SeatNumber { get; set; }
    public Guid? StationId { get; set; }
    public OrderItemStatus Status { get; set; }
    public Guid? KitchenTicketId { get; set; }
    public DateTime? SentToKitchenAt { get; set; }
    public bool IsVoided { get; set; }
    public string? VoidReasonNote { get; set; }
    public Guid? VoidReasonId { get; set; }
    public Guid? VoidedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public bool IsComplimentary { get; set; }
    public int SortOrder { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? ServedAt { get; set; }
    public bool IsRemake { get; set; }
    public Guid? RemakeOfOrderItemId { get; set; }

    public Order Order { get; set; } = null!;
    public MenuItem? MenuItem { get; set; }
    public ItemVariant? Variant { get; set; }
    public Deal? Deal { get; set; }
    public TaxRate? TaxRate { get; set; }
    public Station? Station { get; set; }
    public KitchenTicket? KitchenTicket { get; set; }
    public ReasonCode? VoidReason { get; set; }
    public OrderItem? RemakeOfOrderItem { get; set; }
}
