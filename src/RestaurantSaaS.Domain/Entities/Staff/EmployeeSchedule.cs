using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>EmployeeSchedule: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class EmployeeSchedule : BranchEntity
{
    public Guid EmployeeId { get; set; }
    public Guid WorkShiftId { get; set; }
    public DateOnly WorkDate { get; set; }

    public Employee Employee { get; set; } = null!;
    public WorkShift WorkShift { get; set; } = null!;
}
