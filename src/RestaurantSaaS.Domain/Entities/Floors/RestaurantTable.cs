using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RestaurantTable: branch table. Part of the Floors module. See Database Schema.</summary>
public sealed class RestaurantTable : BranchEntity
{
    public Guid FloorId { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }
    public string? Shape { get; set; }
    public int? PosX { get; set; }
    public int? PosY { get; set; }
    public string? QrToken { get; set; }
    public Guid? CurrentOrderId { get; set; }
    public Guid? MergedIntoTableId { get; set; }

    public Floor Floor { get; set; } = null!;
    public Order? CurrentOrder { get; set; }
    public RestaurantTable? MergedIntoTable { get; set; }
}
