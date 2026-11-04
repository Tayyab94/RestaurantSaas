using RestaurantSaaS.Application.Features.Account;
using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/account/change-password", async (ChangePasswordRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), ct);
                return Results.NoContent();
            })
            .WithTags("Account");

        app.MapGet("/account/preferences", async (IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetMyPreferencesQuery(), ct)))
            .WithTags("Account");

        app.MapPut("/account/preferences", async (UserPreferencesDto request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SaveMyPreferencesCommand(request.Theme, request.Language), ct);
                return Results.NoContent();
            })
            .WithTags("Account");

        return app;
    }
}
