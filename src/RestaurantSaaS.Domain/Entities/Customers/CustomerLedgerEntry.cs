using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CustomerLedgerEntry: branch table. Part of the Customers module. See Database Schema.</summary>
public sealed class CustomerLedgerEntry : BranchEntity
{
    public Guid CustomerId { get; set; }
    public DateOnly EntryDate { get; set; }
    public CustomerLedgerEntryEntryType EntryType { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public Guid? PaymentMethodId { get; set; }
    public string? Notes { get; set; }
    public Guid? ShiftId { get; set; }

    public Customer Customer { get; set; } = null!;
    public PaymentMethod? PaymentMethod { get; set; }
    public Shift? Shift { get; set; }
}
