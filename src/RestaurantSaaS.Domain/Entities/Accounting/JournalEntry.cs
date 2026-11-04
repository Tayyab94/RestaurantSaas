using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>JournalEntry: branch table. Part of the Accounting module. See Database Schema.</summary>
public sealed class JournalEntry : BranchEntity
{
    public string EntryNumber { get; set; } = string.Empty;
    public JournalEntryEntryType EntryType { get; set; }
    public DateOnly EntryDate { get; set; }
    public Guid PeriodId { get; set; }
    public string? SourceType { get; set; }
    public Guid? SourceId { get; set; }
    public string Narration { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public JournalEntryStatus Status { get; set; }
    public Guid? PostedBy { get; set; }
    public Guid? ReversalOfId { get; set; }

    public AccountingPeriod Period { get; set; } = null!;
    public JournalEntry? ReversalOf { get; set; }
}
