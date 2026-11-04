using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>EmployeeLoan: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class EmployeeLoan : BranchEntity
{
    public string LoanNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeLoanLoanType LoanType { get; set; }
    public decimal PrincipalAmount { get; set; }
    public DateOnly IssuedDate { get; set; }
    public int InstallmentCount { get; set; }
    public decimal InstallmentAmount { get; set; }
    public int FirstDeductionMonth { get; set; }
    public int FirstDeductionYear { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LoanStatus Status { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? CashMovementId { get; set; }
    public Guid? PaymentMethodId { get; set; }
    public decimal OutstandingBalance { get; set; }

    public Employee Employee { get; set; } = null!;
    public CashMovement? CashMovement { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}
