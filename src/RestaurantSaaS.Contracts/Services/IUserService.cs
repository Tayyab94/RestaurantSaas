using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Contracts.Services;

public interface IUserService
{
    Task<PagedResult<UserListItemDto>> GetPageAsync(PageRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default);

    Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

    /// <summary>Sets a 4 to 6 digit PIN for quick sign-in. A null or empty PIN removes it.</summary>
    Task SetPinAsync(Guid userId, string? pin, CancellationToken ct = default);

    /// <summary>Gives a user a temporary password; the user must change it at the next sign-in.</summary>
    Task ResetPasswordAsync(Guid userId, string newPassword, CancellationToken ct = default);

    Task SetActiveAsync(Guid userId, bool isActive, CancellationToken ct = default);

    Task<IReadOnlyList<BranchDto>> GetBranchesAsync(CancellationToken ct = default);

    /// <summary>Gives the user another role (a user has one role).</summary>
    Task SetRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default);

    /// <summary>Sets the branches the user may work in. The first one is the default.</summary>
    Task SetBranchesAsync(Guid userId, IReadOnlyList<Guid> branchIds, CancellationToken ct = default);

    Task<PagedResult<LoginHistoryItemDto>> GetLoginHistoryAsync(PageRequest request, CancellationToken ct = default);
}
