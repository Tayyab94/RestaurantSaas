using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TenantFeature: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class TenantFeature : TenantEntity
{
    public Guid FeatureId { get; set; }
    public bool IsEnabled { get; set; }
    public int? LimitValue { get; set; }

    public Feature Feature { get; set; } = null!;
}
