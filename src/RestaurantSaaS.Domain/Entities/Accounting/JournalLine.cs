using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>JournalLine: branch table. Part of the Accounting module. See Database Schema.</summary>
public sealed class JournalLine : BranchEntity
{
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public PartyType? PartyType { get; set; }
    public Guid? PartyId { get; set; }
    public string? Narration { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;
    public ChartOfAccount Account { get; set; } = null!;
}
