using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockRequisitionItem: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockRequisitionItem : BranchEntity
{
    public Guid StockRequisitionId { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid UnitId { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal? ApprovedQuantity { get; set; }
    public decimal? IssuedQuantity { get; set; }
    public decimal? ReceivedQuantity { get; set; }

    public StockRequisition StockRequisition { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
}
