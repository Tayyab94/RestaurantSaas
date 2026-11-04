using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderTable: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderTable : BranchEntity
{
    public Guid OrderId { get; set; }
    public Guid TableId { get; set; }

    public Order Order { get; set; } = null!;
    public RestaurantTable Table { get; set; } = null!;
}
