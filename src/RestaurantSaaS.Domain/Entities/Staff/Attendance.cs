using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Attendance: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class Attendance : BranchEntity
{
    public Guid? ClientRequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly WorkDate { get; set; }
    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    public int WorkedMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    public AttendanceStatus Status { get; set; }
    public AttendanceSource Source { get; set; }

    public Employee Employee { get; set; } = null!;
}
