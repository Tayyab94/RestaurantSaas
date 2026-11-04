using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SubscriptionPayment: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class SubscriptionPayment : TenantEntity
{
    public Guid SubscriptionInvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public DateTime PaidAt { get; set; }

    public SubscriptionInvoice SubscriptionInvoice { get; set; } = null!;
}
