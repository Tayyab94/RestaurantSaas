using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Tip: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class Tip : BranchEntity
{
    public Guid OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid WaiterId { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaidOut { get; set; }
    public DateTime? PaidOutAt { get; set; }
    public Guid? TipPoolId { get; set; }

    public Order Order { get; set; } = null!;
    public Payment? Payment { get; set; }
    public User Waiter { get; set; } = null!;
    public TipPool? TipPool { get; set; }
}
