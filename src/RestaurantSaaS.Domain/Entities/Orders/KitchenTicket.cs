using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>KitchenTicket: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class KitchenTicket : BranchEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid? StationId { get; set; }
    public KitchenTicketStatus Status { get; set; }
    public OrderPriority Priority { get; set; }
    public int? CourseNumber { get; set; }
    public string? TableName { get; set; }
    public Guid WaiterId { get; set; }
    public bool IsReprint { get; set; }
    public Guid? ReprintOfId { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? ServedAt { get; set; }
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? PrintedAt { get; set; }
    public DateTime? ExpectedReadyAt { get; set; }
    public bool IsRemake { get; set; }
    public Guid? RemakeRecordId { get; set; }

    public Order Order { get; set; } = null!;
    public Station? Station { get; set; }
    public User Waiter { get; set; } = null!;
    public KitchenTicket? ReprintOf { get; set; }
    public RemakeRecord? RemakeRecord { get; set; }
}
