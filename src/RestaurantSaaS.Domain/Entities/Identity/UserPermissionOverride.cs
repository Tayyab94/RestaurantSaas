using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>UserPermissionOverride: tenant table. Part of the Identity module. See Database Schema.</summary>
public sealed class UserPermissionOverride : TenantEntity
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsGranted { get; set; }

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
