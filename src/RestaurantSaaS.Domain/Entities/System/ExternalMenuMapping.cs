using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ExternalMenuMapping: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class ExternalMenuMapping : TenantEntity
{
    public IntegrationProvider Provider { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public Guid ExternalItemId { get; set; }
    public string? ExternalName { get; set; }
    public DateTime? LastSyncedAt { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
}
