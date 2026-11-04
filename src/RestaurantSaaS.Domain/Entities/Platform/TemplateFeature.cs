using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TemplateFeature: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class TemplateFeature : BaseEntity
{
    public Guid TemplateId { get; set; }
    public Guid FeatureId { get; set; }
    public bool IsEnabledByDefault { get; set; }

    public BusinessTypeTemplate Template { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
}
