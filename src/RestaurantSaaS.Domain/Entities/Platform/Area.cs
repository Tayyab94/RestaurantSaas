using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Area: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class Area : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public string? City { get; set; }
    public Guid? ParentAreaId { get; set; }
    public bool IsActive { get; set; }

    public Area? ParentArea { get; set; }
}
