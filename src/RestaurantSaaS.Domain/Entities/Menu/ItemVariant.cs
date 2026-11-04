using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ItemVariant: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class ItemVariant : TenantEntity
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
}
