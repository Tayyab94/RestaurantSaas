using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>WorkShift: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class WorkShift : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int BreakMinutes { get; set; }
}
