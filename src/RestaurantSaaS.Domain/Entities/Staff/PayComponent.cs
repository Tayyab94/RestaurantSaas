using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PayComponent: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class PayComponent : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public PayComponentComponentType ComponentType { get; set; }
    public PayComponentCalcType CalcType { get; set; }
    public decimal DefaultValue { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsStatutory { get; set; }
    public bool IsActive { get; set; }
}
