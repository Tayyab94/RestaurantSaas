using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>KitchenPerformanceDaily: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class KitchenPerformanceDaily : BranchEntity
{
    public DateOnly BusinessDate { get; set; }
    public Guid? StationId { get; set; }
    public Guid? MenuItemId { get; set; }
    public int TicketCount { get; set; }
    public int ItemCount { get; set; }
    public int AvgPrepSeconds { get; set; }
    public int P90PrepSeconds { get; set; }
    public int MaxPrepSeconds { get; set; }
    public int LateCount { get; set; }
    public int RemakeCount { get; set; }
    public int? PeakHour { get; set; }
    public DateTime GeneratedAt { get; set; }

    public Station? Station { get; set; }
    public MenuItem? MenuItem { get; set; }
}
