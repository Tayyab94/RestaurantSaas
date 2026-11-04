using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>MessageLog: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class MessageLog : TenantEntity
{
    public NotificationChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? TemplateName { get; set; }
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ProviderReference { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime? SentAt { get; set; }
}
