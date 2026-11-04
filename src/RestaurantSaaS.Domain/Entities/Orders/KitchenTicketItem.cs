using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>KitchenTicketItem: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class KitchenTicketItem : BranchEntity
{
    public Guid KitchenTicketId { get; set; }
    public Guid OrderItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public decimal Quantity { get; set; }
    public string? ModifiersText { get; set; }
    public string? Notes { get; set; }
    public KitchenTicketItemStatus Status { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ReadyAt { get; set; }

    public KitchenTicket KitchenTicket { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
}
