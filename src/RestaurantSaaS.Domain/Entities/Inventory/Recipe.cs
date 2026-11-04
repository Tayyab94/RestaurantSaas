using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Recipe: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class Recipe : TenantEntity
{
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal YieldQuantity { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public string? MethodSummary { get; set; }
    public int? ShelfLifeHours { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
}
