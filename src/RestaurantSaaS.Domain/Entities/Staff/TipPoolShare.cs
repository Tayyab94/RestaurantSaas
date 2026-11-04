using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TipPoolShare: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class TipPoolShare : BranchEntity
{
    public Guid TipPoolId { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal ShareUnits { get; set; }
    public decimal Amount { get; set; }
    public Guid? PayrollItemId { get; set; }

    public TipPool TipPool { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public PayrollItem? PayrollItem { get; set; }
}
