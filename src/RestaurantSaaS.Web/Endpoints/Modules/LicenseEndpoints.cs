using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Licensing;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class LicenseEndpoints
{
    public static IEndpointRouteBuilder MapLicenseOpenEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/license").WithTags("License").AllowAnonymous();

        group.MapGet("/status", async (IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new GetLicenseStatusQuery(), ct)));

        group.MapPost("/activate", async (ActivateLicenseRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new ActivateLicenseCommand(request.Key, request.UserName), ct)));

        group.MapPost("/trial", async (StartTrialRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new StartTrialCommand(request.UserName, request.Phone, request.Email), ct)));

        var server = app.MapGroup("/api/v1/license-server").WithTags("License Server").AllowAnonymous();

        server.MapPost("/activate", async (ServerActivateRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new ServerActivateCommand(request), ct)));

        server.MapPost("/trial", async (ServerTrialRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new ServerTrialCommand(request), ct)));

        return app;
    }

    public static IEndpointRouteBuilder MapLicenseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/license/keys", async (GenerateLicenseKeysRequest request, IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GenerateLicenseKeysCommand(request), ct)))
            .WithTags("License");

        app.MapGet("/license/keys", async ([AsParameters] PageRequest page, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetLicenseKeysPageQuery(page), ct)))
            .WithTags("License");

        app.MapGet("/license/plans", async (IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetPlanOptionsQuery(), ct)))
            .WithTags("License");


        // In MapLicenseOpenEndpoints, ADD after the "/trial" route of `group`:
        group.MapPost("/deactivate", async (IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new DeactivateLicenseCommand(), ct)));

        // ADD after the "/trial" route of `server`:
        server.MapPost("/deactivate", async (ServerDeactivateRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            TypedResults.Ok(await dispatcher.SendAsync(new ServerDeactivateCommand(request), ct)));

        // In MapLicenseEndpoints (secured), ADD before the "/license/plans" route:
        app.MapPost("/license/keys/{id:guid}/reset", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new ResetLicenseKeyCommand(id), ct)))
            .WithTags("License");

        return app;
    }
}
