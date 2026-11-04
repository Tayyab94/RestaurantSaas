using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>UnitConversion: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class UnitConversion : TenantEntity
{
    public Guid FromUnitId { get; set; }
    public Guid ToUnitId { get; set; }
    public decimal Factor { get; set; }

    public Unit FromUnit { get; set; } = null!;
    public Unit ToUnit { get; set; } = null!;
}
