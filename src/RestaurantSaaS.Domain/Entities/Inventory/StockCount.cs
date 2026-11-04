using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockCount: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockCount : BranchEntity
{
    public string CountNumber { get; set; } = string.Empty;
    public DateOnly CountDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CountedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public Guid? LocationId { get; set; }

    public StockLocation? Location { get; set; }
}
