using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Dashboard;
using RestaurantSaaS.Application.Messaging;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/summary", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetDashboardSummaryQuery(), ct)))
            .WithTags("Dashboard");

        return app;
    }
}
