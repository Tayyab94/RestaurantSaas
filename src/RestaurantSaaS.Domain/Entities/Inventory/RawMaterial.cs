using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RawMaterial: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class RawMaterial : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public Guid BaseUnitId { get; set; }
    public Guid? PurchaseUnitId { get; set; }
    public decimal PurchaseToBaseFactor { get; set; }
    public string? Barcode { get; set; }
    public decimal AverageCost { get; set; }
    public decimal LastPurchasePrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public bool TrackExpiry { get; set; }
    public bool IsSellableDirectly { get; set; }
    public bool IsActive { get; set; }
    public decimal YieldPercent { get; set; }
    public Guid? DefaultLocationId { get; set; }
    public bool IsPackaging { get; set; }

    public RawMaterialCategory? Category { get; set; }
    public Unit BaseUnit { get; set; } = null!;
    public Unit? PurchaseUnit { get; set; }
    public StockLocation? DefaultLocation { get; set; }
}
