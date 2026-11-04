using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>FoodSafetyChecklist: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class FoodSafetyChecklist : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public ChecklistCategory Category { get; set; }
    public FoodSafetyChecklistFrequency Frequency { get; set; }
    public bool IsActive { get; set; }
}
