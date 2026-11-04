using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ModifierGroup: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class ModifierGroup : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public int MinSelect { get; set; }
    public int MaxSelect { get; set; }
    public bool IsRequired { get; set; }
}
