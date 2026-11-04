using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PriceList: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class PriceList : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
