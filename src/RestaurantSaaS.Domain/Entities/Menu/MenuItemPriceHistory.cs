using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>MenuItemPriceHistory: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class MenuItemPriceHistory : TenantEntity
{
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public Guid? BranchId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Reason { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
    public Branch? Branch { get; set; }
}
