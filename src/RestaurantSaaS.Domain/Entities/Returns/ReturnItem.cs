using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ReturnItem: branch table. Part of the Returns module. See Database Schema.</summary>
public sealed class ReturnItem : BranchEntity
{
    public Guid ReturnOrderId { get; set; }
    public Guid InvoiceItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public ReturnItemStockAction StockAction { get; set; }
    public string? Note { get; set; }

    public ReturnOrder ReturnOrder { get; set; } = null!;
    public InvoiceItem InvoiceItem { get; set; } = null!;
}
