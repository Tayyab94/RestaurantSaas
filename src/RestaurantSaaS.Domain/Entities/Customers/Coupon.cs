using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Coupon: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class Coupon : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public Guid? PromotionId { get; set; }
    public CouponDiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal? MinOrder { get; set; }
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public bool IsActive { get; set; }

    public Promotion? Promotion { get; set; }
}
