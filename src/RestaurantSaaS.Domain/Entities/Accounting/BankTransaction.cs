using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>BankTransaction: branch table. Part of the Accounting module. See Database Schema.</summary>
public sealed class BankTransaction : BranchEntity
{
    public Guid BankAccountId { get; set; }
    public DateOnly TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? Reference { get; set; }
    public bool IsReconciled { get; set; }
    public DateTime? ReconciledAt { get; set; }
    public Guid? JournalEntryId { get; set; }

    public BankAccount BankAccount { get; set; } = null!;
    public JournalEntry? JournalEntry { get; set; }
}
