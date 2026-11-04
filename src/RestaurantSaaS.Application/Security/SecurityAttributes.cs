namespace RestaurantSaaS.Application.Security;

/// <summary>The caller must have this permission. Several attributes mean all are required.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequirePermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}

/// <summary>
/// The request may run without signing in (sign-in, registration, token refresh).
/// Every other request is blocked for anonymous callers by default.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AllowAnonymousRequestAttribute : Attribute
{
}
