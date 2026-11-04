using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderItemModifier: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderItemModifier : BranchEntity
{
    public Guid OrderItemId { get; set; }
    public Guid? ModifierId { get; set; }
    public string ModifierName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public OrderItem OrderItem { get; set; } = null!;
    public Modifier? Modifier { get; set; }
}
