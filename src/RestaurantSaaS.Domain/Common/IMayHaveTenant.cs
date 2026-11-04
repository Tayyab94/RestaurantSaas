namespace RestaurantSaaS.Domain.Common;

/// <summary>A row that belongs to a tenant OR to the platform (TenantId is null), for example the SuperAdmin user.</summary>
public interface IMayHaveTenant
{
    Guid? TenantId { get; }
}
