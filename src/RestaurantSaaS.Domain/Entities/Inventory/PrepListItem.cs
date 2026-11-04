using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PrepListItem: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class PrepListItem : BranchEntity
{
    public Guid PrepListId { get; set; }
    public Guid? StationId { get; set; }
    public Guid? RecipeId { get; set; }
    public Guid? OutputRawMaterialId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal TargetQuantity { get; set; }
    public decimal OnHandQuantity { get; set; }
    public Guid? UnitId { get; set; }
    public decimal PreparedQuantity { get; set; }
    public PrepListItemStatus Status { get; set; }
    public Guid? AssignedEmployeeId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? DoneAt { get; set; }
    public Guid? ProductionRunId { get; set; }
    public string? Notes { get; set; }

    public PrepList PrepList { get; set; } = null!;
    public Station? Station { get; set; }
    public Recipe? Recipe { get; set; }
    public RawMaterial? OutputRawMaterial { get; set; }
    public Unit? Unit { get; set; }
    public Employee? AssignedEmployee { get; set; }
    public ProductionRun? ProductionRun { get; set; }
}
