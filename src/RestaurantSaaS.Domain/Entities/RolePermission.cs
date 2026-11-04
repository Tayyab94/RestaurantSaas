using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

public sealed class RolePermission : TenantEntity
{
    private RolePermission()
    {
    }

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public Role Role { get; private set; } = null!;

    public Permission Permission { get; private set; } = null!;

    public static RolePermission Create(Guid tenantId, Guid roleId, Guid permissionId) =>
        new() { TenantId = tenantId, RoleId = roleId, PermissionId = permissionId };
}
