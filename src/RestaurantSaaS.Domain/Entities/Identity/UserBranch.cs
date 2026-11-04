using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>UserBranch: tenant table. Part of the Identity module. See Database Schema.</summary>
public sealed class UserBranch : TenantEntity
{
    public Guid UserId { get; set; }
    public Guid BranchId { get; set; }
    public bool IsDefault { get; set; }

    public User User { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
}
