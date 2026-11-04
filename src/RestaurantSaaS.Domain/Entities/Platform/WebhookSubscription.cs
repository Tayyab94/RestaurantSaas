using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>WebhookSubscription: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class WebhookSubscription : TenantEntity
{
    public string EventType { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public string SecretHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? LastStatus { get; set; }
}
