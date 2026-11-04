using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Abstractions;

/// <summary>How this host is deployed (cloud server, on-premise server, or the standalone desktop app).</summary>
public interface IDeploymentInfo
{
    DeploymentMode Mode { get; }
}

public sealed class DeploymentInfo(DeploymentMode mode) : IDeploymentInfo
{
    public DeploymentMode Mode => mode;
}

/// <summary>
/// Request context for hosts that have no HTTP request, such as the standalone desktop app.
/// The identity comes only from the context the Local services put into the scope.
/// </summary>
public sealed class AccessorRequestContext(IRequestContextAccessor accessor) : ICurrentUser, ITenantProvider
{
    private RequestContext? Context => accessor.Current;

    public bool IsAuthenticated => Context?.UserId is not null;

    public Guid? UserId => Context?.UserId;

    public string? UserName => Context?.UserName;

    public Guid? TenantId => Context?.TenantId;

    public bool IsSuperAdmin => Context?.IsSuperAdmin ?? false;

    public bool HasPermission(string permission) =>
        Context is { } context && (context.IsSuperAdmin || context.Permissions.Contains(permission));
}
