using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Unit: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class Unit : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}
