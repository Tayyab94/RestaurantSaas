using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderStatusHistory: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderStatusHistory : BranchEntity
{
    public Guid OrderId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Note { get; set; }

    public Order Order { get; set; } = null!;
}
