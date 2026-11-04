using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Modifier: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class Modifier : TenantEntity
{
    public Guid ModifierGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public decimal Price { get; set; }
    public bool IsDefault { get; set; }
    public Guid? RawMaterialId { get; set; }
    public int SortOrder { get; set; }

    public ModifierGroup ModifierGroup { get; set; } = null!;
    public RawMaterial? RawMaterial { get; set; }
}
