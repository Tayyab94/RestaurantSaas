using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Onboarding;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class OnboardingEndpoints
{
    public static IEndpointRouteBuilder MapOnboardingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/onboarding", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetOnboardingChecklistQuery(), ct)))
            .WithTags("Onboarding");

        app.MapPut("/onboarding/{key}", async (string key, SetOnboardingStepRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SetOnboardingStepCommand(key, request.IsDone), ct);
                return Results.NoContent();
            })
            .WithTags("Onboarding");

        return app;
    }
}
