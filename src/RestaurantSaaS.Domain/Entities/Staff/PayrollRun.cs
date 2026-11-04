using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PayrollRun: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class PayrollRun : BranchEntity
{
    public int PeriodMonth { get; set; }
    public int PeriodYear { get; set; }
    public PayrollRunStatus Status { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
