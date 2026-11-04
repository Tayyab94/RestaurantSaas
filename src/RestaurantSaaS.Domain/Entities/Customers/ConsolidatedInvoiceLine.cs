using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ConsolidatedInvoiceLine: branch table. Part of the Customers module. See Database Schema.</summary>
public sealed class ConsolidatedInvoiceLine : BranchEntity
{
    public Guid ConsolidatedInvoiceId { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }

    public ConsolidatedInvoice ConsolidatedInvoice { get; set; } = null!;
    public Invoice Invoice { get; set; } = null!;
}
