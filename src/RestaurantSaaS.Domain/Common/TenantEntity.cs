namespace RestaurantSaaS.Domain.Common;

/// <summary>A row that belongs to one tenant. The data layer filters and stamps <see cref="TenantId"/> automatically.</summary>
public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
