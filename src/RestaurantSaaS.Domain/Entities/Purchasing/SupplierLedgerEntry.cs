using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierLedgerEntry: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierLedgerEntry : BranchEntity
{
    public Guid SupplierId { get; set; }
    public DateOnly EntryDate { get; set; }
    public SupplierLedgerEntryType EntryType { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }

    public Supplier Supplier { get; set; } = null!;
}
