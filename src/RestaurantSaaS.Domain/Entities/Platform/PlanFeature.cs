using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PlanFeature: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class PlanFeature : BaseEntity
{
    public Guid PlanId { get; set; }
    public Guid FeatureId { get; set; }
    public bool IsIncluded { get; set; }
    public int? LimitValue { get; set; }

    public Plan Plan { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
}
