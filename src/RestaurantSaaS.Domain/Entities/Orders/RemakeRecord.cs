using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RemakeRecord: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class RemakeRecord : BranchEntity
{
    public Guid? ClientRequestId { get; set; }
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid? KitchenTicketId { get; set; }
    public Guid? NewKitchenTicketId { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public int Quantity { get; set; }
    public RemakeRecordRemakeType RemakeType { get; set; }
    public Guid ReasonCodeId { get; set; }
    public string? ReasonNote { get; set; }
    public RemakeRecordChargeTo ChargeTo { get; set; }
    public Guid? StationId { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public decimal CostValue { get; set; }
    public Guid? WastageRecordId { get; set; }
    public DateTime OccurredAt { get; set; }

    public Order Order { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
    public KitchenTicket? KitchenTicket { get; set; }
    public KitchenTicket? NewKitchenTicket { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
    public ReasonCode ReasonCode { get; set; } = null!;
    public Station? Station { get; set; }
    public WastageRecord? WastageRecord { get; set; }
}
