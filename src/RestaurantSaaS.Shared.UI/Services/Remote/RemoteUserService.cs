using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal static class RemoteQuery
{
    public static string For(PageRequest request)
    {
        var query = $"page={request.NormalizedPage}&pageSize={request.NormalizedPageSize}&desc={request.Desc.ToString().ToLowerInvariant()}";

        if (!string.IsNullOrWhiteSpace(request.Sort))
        {
            query += $"&sort={Uri.EscapeDataString(request.Sort)}";
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query += $"&search={Uri.EscapeDataString(request.Search)}";
        }

        return query;
    }
}

internal sealed class RemoteUserService(RemoteApiClient api) : IUserService
{
    public Task<PagedResult<UserListItemDto>> GetPageAsync(PageRequest request, CancellationToken ct = default) =>
        api.GetAsync<PagedResult<UserListItemDto>>($"api/v1/users?{RemoteQuery.For(request)}", ct);

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<RoleDto>>("api/v1/roles", ct);

    public async Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken ct = default) =>
        (await api.PostAsync<Created>("api/v1/users", request, authenticated: true, ct)).Id;

    public Task SetPinAsync(Guid userId, string? pin, CancellationToken ct = default) =>
        api.PostAsync($"api/v1/users/{userId}/pin", new SetPinRequest(pin), authenticated: true, ct);

    public Task ResetPasswordAsync(Guid userId, string newPassword, CancellationToken ct = default) =>
        api.PostAsync($"api/v1/users/{userId}/reset-password", new ResetUserPasswordRequest(newPassword), authenticated: true, ct);

    public Task SetActiveAsync(Guid userId, bool isActive, CancellationToken ct = default) =>
        api.PutAsync($"api/v1/users/{userId}/active", new SetUserActiveRequest(isActive), ct);

    public Task<PagedResult<LoginHistoryItemDto>> GetLoginHistoryAsync(PageRequest request, CancellationToken ct = default) =>
        api.GetAsync<PagedResult<LoginHistoryItemDto>>($"api/v1/login-history?{RemoteQuery.For(request)}", ct);

    public async Task<IReadOnlyList<BranchDto>> GetBranchesAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<BranchDto>>("api/v1/branches", ct);

    public Task SetRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default) =>
        api.PutAsync($"api/v1/users/{userId}/role", new SetUserRoleRequest(roleId), ct);

    public Task SetBranchesAsync(Guid userId, IReadOnlyList<Guid> branchIds, CancellationToken ct = default) =>
        api.PutAsync($"api/v1/users/{userId}/branches", new SetUserBranchesRequest(branchIds), ct);

    private sealed record Created(Guid Id);
}
