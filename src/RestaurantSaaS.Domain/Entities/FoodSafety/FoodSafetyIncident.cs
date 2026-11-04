using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>FoodSafetyIncident: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class FoodSafetyIncident : BranchEntity
{
    public DateOnly IncidentDate { get; set; }
    public FoodSafetyIncidentIncidentType IncidentType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ActionTaken { get; set; }
    public Guid ReportedBy { get; set; }
    public IncidentStatus Status { get; set; }
    public Guid? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid? AttachmentId { get; set; }

    public Attachment? Attachment { get; set; }
}
