using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

public interface IRoleService
{
    /// <summary>All permissions a business role can hold (the platform permissions are not included).</summary>
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<RoleDetailDto>> GetDetailsAsync(CancellationToken ct = default);

    /// <summary>Creates a custom role (Id null) or changes the permissions and limits of an existing one.</summary>
    Task<Guid> SaveAsync(SaveRoleRequest request, CancellationToken ct = default);

    Task DeleteAsync(Guid roleId, CancellationToken ct = default);
}
