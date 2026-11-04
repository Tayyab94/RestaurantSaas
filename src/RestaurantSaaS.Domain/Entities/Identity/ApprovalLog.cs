using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ApprovalLog: branch table. Part of the Identity module. See Database Schema.</summary>
public sealed class ApprovalLog : BranchEntity
{
    public ApprovalLogApprovalType ApprovalType { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid ApprovedBy { get; set; }
    public ApprovalMethod Method { get; set; }
    public string? Reason { get; set; }
    public decimal? Amount { get; set; }
    public DateTime ApprovedAt { get; set; }
    public Guid? DeviceId { get; set; }

    public Device? Device { get; set; }
}
