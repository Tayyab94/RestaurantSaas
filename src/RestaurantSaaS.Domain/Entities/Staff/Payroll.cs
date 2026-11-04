using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Payroll: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class Payroll : BranchEntity
{
    public Guid PayrollRunId { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Overtime { get; set; }
    public decimal Commission { get; set; }
    public decimal Tips { get; set; }
    public decimal Deductions { get; set; }
    public decimal AdvanceRecovered { get; set; }
    public decimal NetPay { get; set; }
    public PayrollStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public Guid? PaymentMethodId { get; set; }

    public PayrollRun PayrollRun { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public PaymentMethod? PaymentMethod { get; set; }
}
