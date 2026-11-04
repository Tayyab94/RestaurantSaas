using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PriceListItem: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class PriceListItem : TenantEntity
{
    public Guid PriceListId { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPercent { get; set; }

    public PriceList PriceList { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
}
