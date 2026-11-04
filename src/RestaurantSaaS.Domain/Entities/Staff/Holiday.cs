using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Holiday: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class Holiday : BranchEntity
{
    public DateOnly HolidayDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}
