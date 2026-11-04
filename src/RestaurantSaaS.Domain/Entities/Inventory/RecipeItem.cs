using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RecipeItem: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class RecipeItem : TenantEntity
{
    public Guid RecipeId { get; set; }
    public Guid? RawMaterialId { get; set; }
    public Guid? SubRecipeId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UnitId { get; set; }
    public decimal WastagePercent { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public RawMaterial? RawMaterial { get; set; }
    public Recipe? SubRecipe { get; set; }
    public Unit Unit { get; set; } = null!;
}
