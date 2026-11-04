using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PurchaseOrder: branch table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class PurchaseOrder : BranchEntity
{
    public string PoNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly? ExpectedDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public Guid? ApprovedBy { get; set; }
    public PurchaseOrderApprovalStatus ApprovalStatus { get; set; }
    public Guid? SourceSuggestionId { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public ReorderSuggestion? SourceSuggestion { get; set; }
}
