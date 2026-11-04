using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RawMaterialCategory: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class RawMaterialCategory : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }

    public RawMaterialCategory? Parent { get; set; }
}
