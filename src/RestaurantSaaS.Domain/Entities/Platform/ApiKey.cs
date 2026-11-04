using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ApiKey: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class ApiKey : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
