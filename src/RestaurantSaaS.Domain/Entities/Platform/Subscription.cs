using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Subscription: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class Subscription : TenantEntity
{
    public Guid PlanId { get; set; }
    public Guid? LicenseId { get; set; }
    public LicenseType LicenseType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public SubscriptionBillingCycle BillingCycle { get; set; }
    public SubscriptionStatus Status { get; set; }
    public bool AutoRenew { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }

    public Plan Plan { get; set; } = null!;
    public License? License { get; set; }
}
