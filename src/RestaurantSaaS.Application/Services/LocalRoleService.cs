using RestaurantSaaS.Application.Features.Roles;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalRoleService(LocalExecutor executor) : IRoleService
{
    public Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetPermissionCatalogQuery(), ct);

    public Task<IReadOnlyList<RoleDetailDto>> GetDetailsAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetRoleDetailsQuery(), ct);

    public Task<Guid> SaveAsync(SaveRoleRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new SaveRoleCommand(request.Id, request.Name, request.Description, request.Permissions, request.Limits), ct);

    public async Task DeleteAsync(Guid roleId, CancellationToken ct = default) =>
        await executor.SendAsync(new DeleteRoleCommand(roleId), ct);
}
