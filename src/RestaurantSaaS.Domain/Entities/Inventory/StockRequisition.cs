using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>StockRequisition: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class StockRequisition : BranchEntity
{
    public string RequisitionNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public StockRequisitionRequisitionType RequisitionType { get; set; }
    public Guid? FromLocationId { get; set; }
    public Guid? ToLocationId { get; set; }
    public Guid? FromBranchId { get; set; }
    public RequisitionStatus Status { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public Guid? IssuedBy { get; set; }
    public Guid? ReceivedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? Notes { get; set; }

    public StockLocation? FromLocation { get; set; }
    public StockLocation? ToLocation { get; set; }
    public Branch? FromBranch { get; set; }
}
