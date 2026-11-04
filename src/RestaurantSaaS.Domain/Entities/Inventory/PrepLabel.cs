using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PrepLabel: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class PrepLabel : BranchEntity
{
    public Guid? RecipeId { get; set; }
    public Guid? RawMaterialId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? QuantityText { get; set; }
    public DateTime PreparedAt { get; set; }
    public DateTime UseByAt { get; set; }
    public Guid PreparedBy { get; set; }
    public DateTime? PrintedAt { get; set; }
    public DateTime? DiscardedAt { get; set; }
    public string? DiscardReason { get; set; }
    public Guid? StockBatchId { get; set; }

    public Recipe? Recipe { get; set; }
    public RawMaterial? RawMaterial { get; set; }
    public StockBatch? StockBatch { get; set; }
}
