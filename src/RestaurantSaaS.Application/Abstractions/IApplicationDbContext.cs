using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Application.Abstractions;

/// <summary>
/// What handlers may do with the database. The identity sets are listed here; every other table of the schema
/// is reached with Set&lt;TEntity&gt;() (for example db.Set&lt;Order&gt;()) or gets its own typed property when its module is built.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }

    DbSet<Branch> Branches { get; }

    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<UserRole> UserRoles { get; }

    DbSet<Permission> Permissions { get; }

    DbSet<RolePermission> RolePermissions { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
