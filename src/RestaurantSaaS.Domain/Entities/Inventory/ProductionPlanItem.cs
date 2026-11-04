using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ProductionPlanItem: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class ProductionPlanItem : BranchEntity
{
    public Guid ProductionPlanId { get; set; }
    public Guid RecipeId { get; set; }
    public Guid OutputRawMaterialId { get; set; }
    public Guid UnitId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; }
    public Guid? ProductionRunId { get; set; }
    public ProductionPlanItemStatus Status { get; set; }

    public ProductionPlan ProductionPlan { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
    public RawMaterial OutputRawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
    public ProductionRun? ProductionRun { get; set; }
}
