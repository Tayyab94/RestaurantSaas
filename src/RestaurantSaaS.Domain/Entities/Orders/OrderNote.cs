using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderNote: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderNote : BranchEntity
{
    public Guid OrderId { get; set; }
    public Guid? OrderItemId { get; set; }
    public string Note { get; set; } = string.Empty;
    public Guid AddedBy { get; set; }

    public Order Order { get; set; } = null!;
    public OrderItem? OrderItem { get; set; }
}
