using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ChecklistItem: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class ChecklistItem : BranchEntity
{
    public Guid ChecklistId { get; set; }
    public int SortOrder { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? TextUrdu { get; set; }
    public ChecklistItemResultType ResultType { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public bool IsRequired { get; set; }

    public FoodSafetyChecklist Checklist { get; set; } = null!;
}
