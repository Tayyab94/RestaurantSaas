using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LoanInstallment: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class LoanInstallment : BranchEntity
{
    public Guid LoanId { get; set; }
    public int InstallmentNumber { get; set; }
    public int DueMonth { get; set; }
    public int DueYear { get; set; }
    public decimal Amount { get; set; }
    public decimal DeductedAmount { get; set; }
    public InstallmentStatus Status { get; set; }
    public Guid? PayrollItemId { get; set; }
    public Guid? RescheduleApprovedBy { get; set; }

    public EmployeeLoan Loan { get; set; } = null!;
    public PayrollItem? PayrollItem { get; set; }
}
