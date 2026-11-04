using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>MenuCategory: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class MenuCategory : TenantEntity
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public string? Code { get; set; }
    public string? ImageUrl { get; set; }
    public string? ColorHex { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    public MenuCategory? Parent { get; set; }
}
