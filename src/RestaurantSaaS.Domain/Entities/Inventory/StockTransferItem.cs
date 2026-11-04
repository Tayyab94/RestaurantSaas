using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockTransferItem: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockTransferItem : TenantEntity
{
    public Guid StockTransferId { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid UnitId { get; set; }
    public decimal SentQuantity { get; set; }
    public decimal? ReceivedQuantity { get; set; }
    public decimal UnitCost { get; set; }

    public StockTransfer StockTransfer { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
}
