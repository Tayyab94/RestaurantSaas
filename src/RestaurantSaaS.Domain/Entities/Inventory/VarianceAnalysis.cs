using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>VarianceAnalysis: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class VarianceAnalysis : BranchEntity
{
    public Guid StockCountId { get; set; }
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public Guid RawMaterialId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal OpeningQuantity { get; set; }
    public decimal PurchasedQuantity { get; set; }
    public decimal NetTransferQuantity { get; set; }
    public decimal TheoreticalUsage { get; set; }
    public decimal ClosingQuantity { get; set; }
    public decimal ActualUsage { get; set; }
    public decimal VarianceQuantity { get; set; }
    public decimal VarianceValue { get; set; }
    public decimal VariancePercent { get; set; }
    public DateTime GeneratedAt { get; set; }

    public StockCount StockCount { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public StockLocation? Location { get; set; }
}
