using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>NumberSequence: branch table. Part of the Platform module. See Database Schema.</summary>
public sealed class NumberSequence : BranchEntity
{
    public string DocumentType { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public long CurrentValue { get; set; }
    public int Padding { get; set; }
    public NumberSequenceResetPolicy ResetPolicy { get; set; }
    public DateOnly? LastResetOn { get; set; }
}
