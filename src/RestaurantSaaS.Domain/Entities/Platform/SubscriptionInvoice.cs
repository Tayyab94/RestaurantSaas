using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SubscriptionInvoice: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class SubscriptionInvoice : TenantEntity
{
    public Guid SubscriptionId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public SubscriptionInvoiceStatus Status { get; set; }
    public DateOnly DueDate { get; set; }
    public DateTime? PaidAt { get; set; }

    public Subscription Subscription { get; set; } = null!;
}
