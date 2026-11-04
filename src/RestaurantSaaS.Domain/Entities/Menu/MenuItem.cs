using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>MenuItem: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class MenuItem : TenantEntity
{
    public Guid CategoryId { get; set; }
    public Guid? StationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public string? KitchenName { get; set; }
    public string? KitchenNameUrdu { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public MenuItemItemType ItemType { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CostPrice { get; set; }
    public Guid? TaxRateId { get; set; }
    public bool PriceIncludesTax { get; set; }
    public MenuItemStockMode StockMode { get; set; }
    public Guid? StockItemId { get; set; }
    public string? Barcode { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public bool AllowDiscount { get; set; }
    public bool IsOpenPrice { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public int SortOrder { get; set; }
    public string? ColorHex { get; set; }
    public bool ShowOnPos { get; set; }
    public bool IsFavorite { get; set; }
    public decimal? TargetFoodCostPercent { get; set; }

    public MenuCategory Category { get; set; } = null!;
    public Station? Station { get; set; }
    public TaxRate? TaxRate { get; set; }
    public RawMaterial? StockItem { get; set; }
}
