using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>WastageRecord: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class WastageRecord : BranchEntity
{
    public Guid? ClientRequestId { get; set; }
    public Guid? RawMaterialId { get; set; }
    public Guid? MenuItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public WastageReason Reason { get; set; }
    public Guid ReportedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime WastedAt { get; set; }
    public Guid? RemakeRecordId { get; set; }

    public RawMaterial? RawMaterial { get; set; }
    public MenuItem? MenuItem { get; set; }
    public RemakeRecord? RemakeRecord { get; set; }
}
