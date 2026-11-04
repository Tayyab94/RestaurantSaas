using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderDiscount: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderDiscount : BranchEntity
{
    public Guid OrderId { get; set; }
    public Guid? OrderItemId { get; set; }
    public OrderDiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid? CouponId { get; set; }
    public Guid? ApprovedBy { get; set; }

    public Order Order { get; set; } = null!;
    public OrderItem? OrderItem { get; set; }
    public Promotion? Promotion { get; set; }
    public Coupon? Coupon { get; set; }
}
