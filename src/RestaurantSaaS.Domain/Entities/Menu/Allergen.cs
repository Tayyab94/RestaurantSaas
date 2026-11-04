using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Allergen: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class Allergen : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public string IconKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
