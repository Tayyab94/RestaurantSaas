using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ChecklistRun: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class ChecklistRun : BranchEntity
{
    public Guid ChecklistId { get; set; }
    public DateOnly RunDate { get; set; }
    public Guid? ShiftId { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ChecklistRunStatus Status { get; set; }
    public Guid? SignedBy { get; set; }

    public FoodSafetyChecklist Checklist { get; set; } = null!;
    public Shift? Shift { get; set; }
}
