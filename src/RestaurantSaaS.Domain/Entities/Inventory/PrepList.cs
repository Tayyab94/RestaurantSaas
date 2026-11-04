using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PrepList: branch table. Part of the Inventory module. See Database Schema.</summary>
public sealed class PrepList : BranchEntity
{
    public DateOnly BusinessDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public PrepListStatus Status { get; set; }
    public PrepListSource GeneratedFrom { get; set; }
    public int BasisWeeks { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public Guid? PublishedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
}
