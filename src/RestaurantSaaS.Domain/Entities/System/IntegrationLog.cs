using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>IntegrationLog: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class IntegrationLog : TenantEntity
{
    public IntegrationProvider Provider { get; set; }
    public IntegrationLogDirection Direction { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public Guid? ExternalId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RequestExcerpt { get; set; }
    public string? ResponseExcerpt { get; set; }
    public DateTime OccurredAt { get; set; }
}
