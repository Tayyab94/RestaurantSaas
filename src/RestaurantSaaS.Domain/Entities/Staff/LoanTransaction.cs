using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LoanTransaction: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class LoanTransaction : BranchEntity
{
    public Guid LoanId { get; set; }
    public DateOnly TransactionDate { get; set; }
    public LoanTransactionTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public Guid? PayrollItemId { get; set; }
    public string? Notes { get; set; }

    public EmployeeLoan Loan { get; set; } = null!;
    public PayrollItem? PayrollItem { get; set; }
}
