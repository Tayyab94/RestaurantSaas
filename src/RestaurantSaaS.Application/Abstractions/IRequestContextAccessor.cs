namespace RestaurantSaaS.Application.Abstractions;

/// <summary>The identity of one request: user, tenant and permissions.</summary>
public sealed record RequestContext(
    Guid? UserId,
    string? UserName,
    Guid? TenantId,
    bool IsSuperAdmin,
    IReadOnlySet<string> Permissions);

/// <summary>
/// Lets in-process (Local) services run a handler on behalf of the UI session.
/// For API calls the context comes from the JWT instead.
/// </summary>
public interface IRequestContextAccessor
{
    RequestContext? Current { get; set; }
}

internal sealed class RequestContextAccessor : IRequestContextAccessor
{
    public RequestContext? Current { get; set; }
}
