using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth").AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new LoginCommand(request.BusinessCode, request.UserName, request.Password), ct)));

        group.MapPost("/refresh", async (RefreshRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new RefreshTokenCommand(request.RefreshToken), ct)));

        group.MapPost("/logout", async (LogoutRequest request, IDispatcher dispatcher, CancellationToken ct) =>
        {
            await dispatcher.SendAsync(new LogoutCommand(request.RefreshToken), ct);
            return Results.NoContent();
        });

        group.MapPost("/register", async (RegisterBusinessRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(
                new RegisterBusinessCommand(request.BusinessName, request.BusinessType, request.OwnerFullName, request.UserName, request.Email, request.Phone, request.Password),
                ct)));

        group.MapPost("/pin-login", async (PinLoginRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new PinLoginCommand(request.BusinessCode, request.UserName, request.Pin), ct)));

        group.MapPost("/forgot-password", async (RequestPasswordResetRequest request, IDispatcher dispatcher, CancellationToken ct) =>
        {
            await dispatcher.SendAsync(new RequestPasswordResetCommand(request.BusinessCode, request.UserName), ct);
            return Results.NoContent();
        });

        group.MapPost("/reset-password", async (ResetPasswordRequest request, IDispatcher dispatcher, CancellationToken ct) =>
        {
            await dispatcher.SendAsync(new ResetPasswordCommand(request.BusinessCode, request.UserName, request.Code, request.NewPassword), ct);
            return Results.NoContent();
        });

        // Open to everyone: does this installation still need its first-run setup?
        app.MapGet("/api/v1/setup/status", async (IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new RestaurantSaaS.Application.Features.Setup.GetSetupStatusQuery(), ct)))
            .WithTags("Setup")
            .AllowAnonymous();

        return app;
    }
}
