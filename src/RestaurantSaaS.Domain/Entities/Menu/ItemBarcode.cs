using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ItemBarcode: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class ItemBarcode : TenantEntity
{
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public decimal? PackSize { get; set; }
    public bool IsPrimary { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
}
