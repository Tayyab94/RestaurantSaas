using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>MenuItemAllergen: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class MenuItemAllergen : TenantEntity
{
    public Guid MenuItemId { get; set; }
    public Guid AllergenId { get; set; }
    public AllergenPresence Presence { get; set; }
    public AllergenSource Source { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public Allergen Allergen { get; set; } = null!;
}
