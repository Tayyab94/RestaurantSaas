using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Users;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async ([AsParameters] PageRequest page, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetUsersPageQuery(page), ct)))
            .WithTags("Users");

        app.MapGet("/roles", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetRolesQuery(), ct)))
            .WithTags("Users");

        app.MapPost("/users", async (CreateUserRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.SendAsync(
                    new CreateUserCommand(request.UserName, request.FullName, request.Email, request.Phone, request.Password, request.RoleId, request.BranchIds ?? []),
                    ct);
                return Results.Created($"/api/v1/users/{id}", new { id });
            })
            .WithTags("Users");

        app.MapPost("/users/{id:guid}/pin", async (Guid id, SetPinRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SetUserPinCommand(id, request.Pin), ct);
                return Results.NoContent();
            })
            .WithTags("Users");

        app.MapPost("/users/{id:guid}/reset-password", async (Guid id, ResetUserPasswordRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new ResetUserPasswordCommand(id, request.NewPassword), ct);
                return Results.NoContent();
            })
            .WithTags("Users");

        app.MapPut("/users/{id:guid}/active", async (Guid id, SetUserActiveRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SetUserActiveCommand(id, request.IsActive), ct);
                return Results.NoContent();
            })
            .WithTags("Users");

        app.MapGet("/branches", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetBranchesQuery(), ct)))
            .WithTags("Users");

        app.MapPut("/users/{id:guid}/role", async (Guid id, SetUserRoleRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SetUserRoleCommand(id, request.RoleId), ct);
                return Results.NoContent();
            })
            .WithTags("Users");

        app.MapPut("/users/{id:guid}/branches", async (Guid id, SetUserBranchesRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new SetUserBranchesCommand(id, request.BranchIds ?? []), ct);
                return Results.NoContent();
            })
            .WithTags("Users");

        app.MapGet("/login-history", async ([AsParameters] PageRequest page, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetLoginHistoryPageQuery(page), ct)))
            .WithTags("Users");

        return app;
    }
}
