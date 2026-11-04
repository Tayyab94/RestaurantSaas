using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>InvoiceTax: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class InvoiceTax : BranchEntity
{
    public Guid InvoiceId { get; set; }
    public Guid? TaxRateId { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public TaxRate? TaxRate { get; set; }
}
