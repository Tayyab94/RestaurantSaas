using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockAdjustment: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockAdjustment : BranchEntity
{
    public string AdjustmentNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateOnly AdjustmentDate { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string Status { get; set; } = string.Empty;
}
