using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CouponRedemption: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class CouponRedemption : TenantEntity
{
    public Guid CouponId { get; set; }
    public Guid OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Amount { get; set; }

    public Coupon Coupon { get; set; } = null!;
    public Order Order { get; set; } = null!;
    public Customer? Customer { get; set; }
}
