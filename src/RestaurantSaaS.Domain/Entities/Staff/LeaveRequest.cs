using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LeaveRequest: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class LeaveRequest : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string? Reason { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public Guid? ApprovedBy { get; set; }

    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}
