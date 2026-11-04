using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RawMaterialAllergen: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class RawMaterialAllergen : TenantEntity
{
    public Guid RawMaterialId { get; set; }
    public Guid AllergenId { get; set; }
    public AllergenPresence Presence { get; set; }

    public RawMaterial RawMaterial { get; set; } = null!;
    public Allergen Allergen { get; set; } = null!;
}
