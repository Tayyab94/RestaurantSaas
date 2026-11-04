using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Station: branch table. Part of the Menu module. See Database Schema.</summary>
public sealed class Station : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public Guid? PrinterId { get; set; }
    public bool KdsEnabled { get; set; }
    public int SortOrder { get; set; }
    public int? TargetPrepSeconds { get; set; }

    public Printer? Printer { get; set; }
}
