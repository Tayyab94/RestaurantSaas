using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Deal: tenant table. Part of the Menu module. See Database Schema.</summary>
public sealed class Deal : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public string? Code { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public string? DaysOfWeek { get; set; }
    public bool IsActive { get; set; }
}
