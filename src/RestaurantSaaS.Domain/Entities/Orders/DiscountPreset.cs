using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DiscountPreset: tenant table. Part of the Orders module. See Database Schema.</summary>
public sealed class DiscountPreset : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public DiscountPresetType DiscountType { get; set; }
    public decimal Value { get; set; }
    public DiscountAppliesTo AppliesTo { get; set; }
    public bool RequiresApproval { get; set; }
    public decimal? MaxValue { get; set; }
    public bool IsActive { get; set; }
}
