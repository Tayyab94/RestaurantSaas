using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteRoleService(RemoteApiClient api) : IRoleService
{
    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<PermissionDto>>("api/v1/permissions", ct);

    public async Task<IReadOnlyList<RoleDetailDto>> GetDetailsAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<RoleDetailDto>>("api/v1/roles/details", ct);

    public async Task<Guid> SaveAsync(SaveRoleRequest request, CancellationToken ct = default) =>
        (await api.PostAsync<Created>("api/v1/roles", request, authenticated: true, ct)).Id;

    public Task DeleteAsync(Guid roleId, CancellationToken ct = default) =>
        api.DeleteAsync($"api/v1/roles/{roleId}", ct);

    private sealed record Created(Guid Id);
}
