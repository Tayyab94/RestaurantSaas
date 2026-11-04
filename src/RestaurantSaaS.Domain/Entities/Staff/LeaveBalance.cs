using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LeaveBalance: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class LeaveBalance : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal Entitled { get; set; }
    public decimal Taken { get; set; }
    public decimal Balance { get; set; }

    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}
