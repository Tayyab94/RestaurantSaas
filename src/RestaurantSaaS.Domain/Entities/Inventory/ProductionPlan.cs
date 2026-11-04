using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ProductionPlan: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class ProductionPlan : BranchEntity
{
    public DateOnly PlanDate { get; set; }
    public Guid CentralBranchId { get; set; }
    public ProductionPlanStatus Status { get; set; }
    public ProductionPlanSource GeneratedFrom { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Branch CentralBranch { get; set; } = null!;
}
