using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>MessageTemplate: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class MessageTemplate : TenantEntity
{
    public NotificationChannel Channel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
