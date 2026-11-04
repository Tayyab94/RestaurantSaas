using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PayAdjustment: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class PayAdjustment : BranchEntity
{
    public Guid EmployeeId { get; set; }
    public PayAdjustmentAdjustmentType AdjustmentType { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int EffectiveMonth { get; set; }
    public int EffectiveYear { get; set; }
    public Guid? ApprovedBy { get; set; }
    public PayAdjustmentStatus Status { get; set; }
    public Guid? PayrollItemId { get; set; }

    public Employee Employee { get; set; } = null!;
    public PayrollItem? PayrollItem { get; set; }
}
