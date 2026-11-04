using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>InvoiceItem: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class InvoiceItem : BranchEntity
{
    public Guid InvoiceId { get; set; }
    public Guid? OrderItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRatePercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public decimal CostPrice { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public OrderItem? OrderItem { get; set; }
}
