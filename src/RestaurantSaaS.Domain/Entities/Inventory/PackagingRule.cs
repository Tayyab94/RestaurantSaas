using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PackagingRule: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class PackagingRule : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public PackagingOrderType OrderType { get; set; }
    public PackagingAppliesTo AppliesTo { get; set; }
    public Guid? MenuItemId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid RawMaterialId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UnitId { get; set; }
    public bool IsActive { get; set; }

    public MenuItem? MenuItem { get; set; }
    public MenuCategory? Category { get; set; }
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
}
