using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

public sealed class UserRole : TenantEntity
{
    private UserRole()
    {
    }

    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    public User User { get; private set; } = null!;

    public Role Role { get; private set; } = null!;

    public static UserRole Create(Guid tenantId, Guid userId, Guid roleId) =>
        new() { TenantId = tenantId, UserId = userId, RoleId = roleId };
}
