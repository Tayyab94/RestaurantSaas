using RestaurantSaaS.Web.Endpoints.Modules;

namespace RestaurantSaaS.Web.Endpoints;

/// <summary>
/// REST API (version 1), hosted in this same project. Endpoints stay thin: they translate HTTP to a request
/// and send it to the dispatcher. Add one static class per module in Endpoints/Modules and call it here.
/// </summary>
public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Sign-in, registration and token refresh are open; the handlers themselves decide what is allowed.
        app.MapAuthEndpoints();

        // Everything else needs a valid token (and, inside the handlers, the right permission).
        var secured = app.MapGroup("/api/v1").RequireAuthorization();
        secured.MapAccountEndpoints();
        secured.MapUserEndpoints();
        secured.MapTenantEndpoints();
        secured.MapDashboardEndpoints();
        secured.MapOnboardingEndpoints();
        secured.MapRoleEndpoints();

        return app;
    }
}
