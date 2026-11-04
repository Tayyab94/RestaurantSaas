using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockCountItem: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockCountItem : BranchEntity
{
    public Guid StockCountId { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal CountedQuantity { get; set; }
    public decimal Difference { get; set; }

    public StockCount StockCount { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public StockLocation? Location { get; set; }
}
