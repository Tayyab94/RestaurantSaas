   namespace RestaurantSaaS.Application.Abstractions;

/// <summary>The tenant of the current request or session. Null means "no tenant": tenant-owned data is invisible.</summary>
public interface ITenantProvider
{
    Guid? TenantId { get; }
}
