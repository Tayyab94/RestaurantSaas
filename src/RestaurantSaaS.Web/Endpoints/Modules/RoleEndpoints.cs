using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Features.Roles;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Web.Endpoints.Modules;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/permissions", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetPermissionCatalogQuery(), ct)))
            .WithTags("Roles");

        app.MapGet("/roles/details", async ([FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetRoleDetailsQuery(), ct)))
            .WithTags("Roles");

        app.MapPost("/roles", async (SaveRoleRequest request, IDispatcher dispatcher, CancellationToken ct) =>
            {
                var id = await dispatcher.SendAsync(
                    new SaveRoleCommand(request.Id, request.Name, request.Description, request.Permissions ?? [], request.Limits), ct);
                return Results.Ok(new { id });
            })
            .WithTags("Roles");

        app.MapDelete("/roles/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
            {
                await dispatcher.SendAsync(new DeleteRoleCommand(id), ct);
                return Results.NoContent();
            })
            .WithTags("Roles");

        app.MapPost("/approvals", async (ApprovalRequest request, IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(
                    new RequestApprovalCommand(
                        request.ApprovalType, request.BranchId, request.ApproverUserName, request.Pin,
                        request.ReferenceType, request.ReferenceId, request.Amount, request.Percent, request.Reason),
                    ct)))
            .WithTags("Approvals");

        app.MapGet("/audit-log", async ([AsParameters] PageRequest page, [FromServices] IDispatcher dispatcher, CancellationToken ct) =>
                TypedResults.Ok(await dispatcher.SendAsync(new GetAuditLogPageQuery(page), ct)))
            .WithTags("Audit");

        return app;
    }
}
