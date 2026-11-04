using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Cheque: branch table. Part of the Accounting module. See Database Schema.</summary>
public sealed class Cheque : BranchEntity
{
    public string ChequeNumber { get; set; } = string.Empty;
    public ChequeDirection Direction { get; set; }
    public PartyType PartyType { get; set; }
    public Guid? PartyId { get; set; }
    public Guid? BankAccountId { get; set; }
    public string? BankName { get; set; }
    public decimal Amount { get; set; }
    public DateOnly ChequeDate { get; set; }
    public ChequeStatus Status { get; set; }
    public DateOnly? ClearedDate { get; set; }
    public Guid? JournalEntryId { get; set; }

    public BankAccount? BankAccount { get; set; }
    public JournalEntry? JournalEntry { get; set; }
}
