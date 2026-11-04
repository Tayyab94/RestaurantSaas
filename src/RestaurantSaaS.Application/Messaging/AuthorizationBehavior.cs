using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Security;

namespace RestaurantSaaS.Application.Messaging;

/// <summary>
/// Default deny: every request needs a signed-in user unless it is marked [AllowAnonymousRequest],
/// and every [RequirePermission] must be satisfied. SuperAdmin passes all permission checks.
/// This is enforced here, on the server side, whatever the UI shows.
/// </summary>
internal sealed class AuthorizationBehavior<TRequest, TResponse>(ICurrentUser user)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly bool Anonymous = typeof(TRequest).IsDefined(typeof(AllowAnonymousRequestAttribute), inherit: true);

    private static readonly string[] Required = typeof(TRequest)
        .GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
        .Cast<RequirePermissionAttribute>()
        .Select(a => a.Permission)
        .ToArray();

    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!Anonymous)
        {
            if (!user.IsAuthenticated)
            {
                throw new UnauthorizedException("unauthenticated", "Please sign in to continue.");
            }

            foreach (var permission in Required)
            {
                if (!user.HasPermission(permission))
                {
                    throw new ForbiddenException($"You do not have the permission '{permission}'.");
                }
            }
        }

        return next();
    }
}
