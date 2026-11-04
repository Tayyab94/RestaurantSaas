using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockMovement: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockMovement : BranchEntity
{
    public Guid RawMaterialId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? RequisitionId { get; set; }
    public StockMovementMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceAfter { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string? Reason { get; set; }
    public DateTime MovementDate { get; set; }
    public Guid PerformedBy { get; set; }

    public RawMaterial RawMaterial { get; set; } = null!;
    public StockBatch? Batch { get; set; }
    public StockLocation? Location { get; set; }
    public StockRequisition? Requisition { get; set; }
}
