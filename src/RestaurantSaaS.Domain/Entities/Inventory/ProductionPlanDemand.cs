using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ProductionPlanDemand: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class ProductionPlanDemand : BranchEntity
{
    public Guid ProductionPlanItemId { get; set; }
    public Guid RequestingBranchId { get; set; }
    public Guid? StockRequisitionId { get; set; }
    public decimal Quantity { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public Guid? DispatchTransferId { get; set; }
    public ProductionDemandStatus Status { get; set; }

    public ProductionPlanItem ProductionPlanItem { get; set; } = null!;
    public Branch RequestingBranch { get; set; } = null!;
    public StockRequisition? StockRequisition { get; set; }
    public StockTransfer? DispatchTransfer { get; set; }
}
