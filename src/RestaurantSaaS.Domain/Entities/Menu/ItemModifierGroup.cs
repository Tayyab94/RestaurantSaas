using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ItemModifierGroup: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class ItemModifierGroup : TenantEntity
{
    public Guid MenuItemId { get; set; }
    public Guid ModifierGroupId { get; set; }
    public int SortOrder { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ModifierGroup ModifierGroup { get; set; } = null!;
}
