using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Auth;

/// <summary>Builds the signed-in user's profile with roles, permissions and branches (used by sign-in and token refresh).</summary>
internal sealed class UserSessionBuilder(IApplicationDbContext db)
{
    public async Task<CurrentUserDto> BuildAsync(User user, Tenant? tenant, CancellationToken ct)
    {
        var roles = await db.UserRoles.IgnoreQueryFilters()
            .Where(ur => !ur.IsDeleted && ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .OrderBy(name => name)
            .ToListAsync(ct);

        List<string> permissions;

        if (user.IsSuperAdmin)
        {
            permissions = Permissions.All.Select(p => p.Key).ToList();
        }
        else
        {
            permissions = await db.RolePermissions.IgnoreQueryFilters()
                .Where(rp => !rp.IsDeleted
                             && db.UserRoles.IgnoreQueryFilters().Any(ur => !ur.IsDeleted && ur.UserId == user.Id && ur.RoleId == rp.RoleId))
                .Select(rp => rp.Permission.Key)
                .Distinct()
                .OrderBy(key => key)
                .ToListAsync(ct);
        }

        var branches = await LoadBranchesAsync(user, roles, ct);

        return new CurrentUserDto(
            user.Id,
            user.UserName,
            user.FullName,
            user.TenantId,
            tenant?.Name,
            tenant?.Code,
            user.IsSuperAdmin,
            user.MustChangePassword,
            roles,
            permissions,
            branches);
    }

    // The Owner may work in every branch; everyone else only in the branches they were given.
    private async Task<List<BranchAccessDto>> LoadBranchesAsync(User user, List<string> roleNames, CancellationToken ct)
    {
        if (user.TenantId is null)
        {
            return [];
        }

        var all = await db.Branches.IgnoreQueryFilters()
            .Where(b => !b.IsDeleted && b.TenantId == user.TenantId)
            .OrderByDescending(b => b.IsMain).ThenBy(b => b.Name)
            .Select(b => new { b.Id, b.Name, b.IsMain })
            .ToListAsync(ct);

        if (roleNames.Contains(SystemRoles.Owner))
        {
            return all.Select(b => new BranchAccessDto(b.Id, b.Name, b.IsMain)).ToList();
        }

        var assigned = await db.Set<UserBranch>().IgnoreQueryFilters()
            .Where(ub => !ub.IsDeleted && ub.UserId == user.Id)
            .Select(ub => new { ub.BranchId, ub.IsDefault })
            .ToListAsync(ct);

        return all
            .Where(b => assigned.Any(a => a.BranchId == b.Id))
            .Select(b => new BranchAccessDto(b.Id, b.Name, assigned.Any(a => a.BranchId == b.Id && a.IsDefault)))
            .ToList();
    }
}
