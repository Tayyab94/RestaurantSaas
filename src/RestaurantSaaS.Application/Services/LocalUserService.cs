using RestaurantSaaS.Application.Features.Users;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalUserService(LocalExecutor executor) : IUserService
{
    public Task<PagedResult<UserListItemDto>> GetPageAsync(PageRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new GetUsersPageQuery(request), ct);

    public Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetRolesQuery(), ct);

    public Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new CreateUserCommand(request.UserName, request.FullName, request.Email, request.Phone, request.Password, request.RoleId, request.BranchIds ?? []),
            ct);

    public async Task SetPinAsync(Guid userId, string? pin, CancellationToken ct = default) =>
        await executor.SendAsync(new SetUserPinCommand(userId, pin), ct);

    public async Task ResetPasswordAsync(Guid userId, string newPassword, CancellationToken ct = default) =>
        await executor.SendAsync(new ResetUserPasswordCommand(userId, newPassword), ct);

    public async Task SetActiveAsync(Guid userId, bool isActive, CancellationToken ct = default) =>
        await executor.SendAsync(new SetUserActiveCommand(userId, isActive), ct);

    public Task<IReadOnlyList<BranchDto>> GetBranchesAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetBranchesQuery(), ct);

    public async Task SetRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default) =>
        await executor.SendAsync(new SetUserRoleCommand(userId, roleId), ct);

    public async Task SetBranchesAsync(Guid userId, IReadOnlyList<Guid> branchIds, CancellationToken ct = default) =>
        await executor.SendAsync(new SetUserBranchesCommand(userId, branchIds), ct);

    public Task<PagedResult<LoginHistoryItemDto>> GetLoginHistoryAsync(PageRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new GetLoginHistoryPageQuery(request), ct);
}
