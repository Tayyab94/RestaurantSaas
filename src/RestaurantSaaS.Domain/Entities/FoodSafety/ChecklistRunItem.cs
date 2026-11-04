using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ChecklistRunItem: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class ChecklistRunItem : BranchEntity
{
    public Guid ChecklistRunId { get; set; }
    public Guid ChecklistItemId { get; set; }
    public bool? ResultYesNo { get; set; }
    public decimal? ResultNumber { get; set; }
    public string? ResultText { get; set; }
    public Guid? AttachmentId { get; set; }
    public bool IsPass { get; set; }
    public string? Note { get; set; }

    public ChecklistRun ChecklistRun { get; set; } = null!;
    public ChecklistItem ChecklistItem { get; set; } = null!;
    public Attachment? Attachment { get; set; }
}
