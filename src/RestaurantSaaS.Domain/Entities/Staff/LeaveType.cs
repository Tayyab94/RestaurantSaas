using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LeaveType: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class LeaveType : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public int DaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public bool CanCarryForward { get; set; }
    public bool IsActive { get; set; }
}
