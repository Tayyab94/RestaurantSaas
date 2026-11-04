using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>WebhookDelivery: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class WebhookDelivery : TenantEntity
{
    public Guid WebhookSubscriptionId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public int? ResponseCode { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public WebhookSubscription WebhookSubscription { get; set; } = null!;
}
