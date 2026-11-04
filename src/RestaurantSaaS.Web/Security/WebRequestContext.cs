using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Web.Security;

/// <summary>
/// Who is calling and for which tenant. For API calls it reads the JWT claims; for in-process calls made by the
/// Blazor UI it uses the context the Local service put into the scope (IRequestContextAccessor).
/// </summary>
internal sealed class WebRequestContext(IRequestContextAccessor accessor, IHttpContextAccessor http)
    : ICurrentUser, ITenantProvider
{
    private RequestContext? _fromClaims;
    private bool _claimsRead;

    private RequestContext? Context => accessor.Current ?? FromClaims();

    public bool IsAuthenticated => Context?.UserId is not null;

    public Guid? UserId => Context?.UserId;

    public string? UserName => Context?.UserName;

    public Guid? TenantId => Context?.TenantId;

    public bool IsSuperAdmin => Context?.IsSuperAdmin ?? false;

    public bool HasPermission(string permission) =>
        Context is { } context && (context.IsSuperAdmin || context.Permissions.Contains(permission));

    private RequestContext? FromClaims()
    {
        if (_claimsRead)
        {
            return _fromClaims;
        }

        _claimsRead = true;
        var principal = http.HttpContext?.User;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        Guid? userId = Guid.TryParse(principal.FindFirst("sub")?.Value, out var u) ? u : null;
        Guid? tenantId = Guid.TryParse(principal.FindFirst("tenant_id")?.Value, out var t) ? t : null;
        var permissions = principal.FindAll("perm").Select(c => c.Value).ToHashSet();

        _fromClaims = new RequestContext(
            userId,
            principal.FindFirst("name")?.Value,
            tenantId,
            principal.FindFirst("is_super_admin")?.Value == "true",
            permissions);

        return _fromClaims;
    }
}
