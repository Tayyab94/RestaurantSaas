using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ProductionRun: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class ProductionRun : BranchEntity
{
    public Guid? RecipeId { get; set; }
    public Guid? OutputRawMaterialId { get; set; }
    public decimal Quantity { get; set; }
    public Guid? UnitId { get; set; }
    public DateTime ProducedAt { get; set; }
    public Guid ProducedBy { get; set; }
    public decimal TotalCost { get; set; }
    public string? Notes { get; set; }
    public Guid? PrepListItemId { get; set; }
    public Guid? ProductionPlanItemId { get; set; }

    public Recipe? Recipe { get; set; }
    public RawMaterial? OutputRawMaterial { get; set; }
    public Unit? Unit { get; set; }
    public PrepListItem? PrepListItem { get; set; }
    public ProductionPlanItem? ProductionPlanItem { get; set; }
}
