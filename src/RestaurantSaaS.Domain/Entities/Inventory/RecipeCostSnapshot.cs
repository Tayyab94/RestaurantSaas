using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RecipeCostSnapshot: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class RecipeCostSnapshot : TenantEntity
{
    public Guid RecipeId { get; set; }
    public Guid MenuItemId { get; set; }
    public decimal CostPerPortion { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal FoodCostPercent { get; set; }
    public decimal? TargetFoodCostPercent { get; set; }
    public DateTime ComputedAt { get; set; }
    public RecipeCostSnapshotTrigger Trigger { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}
