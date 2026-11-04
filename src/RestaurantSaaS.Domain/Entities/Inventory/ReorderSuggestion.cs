using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ReorderSuggestion: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class ReorderSuggestion : BranchEntity
{
    public Guid RawMaterialId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal CurrentQuantity { get; set; }
    public decimal ParLevel { get; set; }
    public decimal SuggestedQuantity { get; set; }
    public Guid? SuggestedSupplierId { get; set; }
    public ReorderSuggestionReason Reason { get; set; }
    public ReorderSuggestionStatus Status { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public DateTime GeneratedAt { get; set; }

    public RawMaterial RawMaterial { get; set; } = null!;
    public StockLocation? Location { get; set; }
    public Supplier? SuggestedSupplier { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
}
