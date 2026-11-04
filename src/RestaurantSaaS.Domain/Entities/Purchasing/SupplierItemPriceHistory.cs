using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierItemPriceHistory: tenant table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierItemPriceHistory : TenantEntity
{
    public Guid SupplierItemId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangedAt { get; set; }
    public Guid ChangedBy { get; set; }
    public SupplierItemPriceHistorySource Source { get; set; }

    public SupplierItem SupplierItem { get; set; } = null!;
}
