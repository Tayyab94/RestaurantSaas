using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Tenants;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tenants", async ([AsParameters] PageRequest page, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetTenantsPageQuery(page), ct)))
            .WithTags("Tenants");

        return app;
    }
}
