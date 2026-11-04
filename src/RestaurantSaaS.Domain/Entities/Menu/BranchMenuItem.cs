using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>BranchMenuItem: branch table. Part of the Menu module. See Database Schema.</summary>
public sealed class BranchMenuItem : BranchEntity
{
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsActive { get; set; }
    public DateTime? OutUntil { get; set; }
    public string? OutReason { get; set; }
    public BranchMenuItemOutSource? OutSource { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
}
