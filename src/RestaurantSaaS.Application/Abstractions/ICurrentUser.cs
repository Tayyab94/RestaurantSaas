namespace RestaurantSaaS.Application.Abstractions;

/// <summary>Who is calling. Built from the API token, or from the UI session for in-process calls.</summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    string? UserName { get; }

    Guid? TenantId { get; }

    bool IsSuperAdmin { get; }

    bool HasPermission(string permission);
}
