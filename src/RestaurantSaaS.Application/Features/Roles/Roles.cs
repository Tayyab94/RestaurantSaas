using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Roles;

internal static class RoleRules
{
    public static bool IsOwner(Role role) => role.IsSystem && role.Name == SystemRoles.Owner;
}

[RequirePermission(Permissions.RoleView)]
public sealed record GetPermissionCatalogQuery : IRequest<IReadOnlyList<PermissionDto>>;

internal sealed class GetPermissionCatalogHandler : IRequestHandler<GetPermissionCatalogQuery, IReadOnlyList<PermissionDto>>
{
    public Task<IReadOnlyList<PermissionDto>> HandleAsync(GetPermissionCatalogQuery request, CancellationToken ct)
    {
        IReadOnlyList<PermissionDto> list = Permissions.All
            .Where(p => p.Module != Permissions.PlatformModule)
            .Select(p => new PermissionDto(p.Key, p.Module, p.Description))
            .ToList();

        return Task.FromResult(list);
    }
}

/// <summary>Every role of the business with its permissions, number of users and approval limits.</summary>
[RequirePermission(Permissions.RoleView)]
public sealed record GetRoleDetailsQuery : IRequest<IReadOnlyList<RoleDetailDto>>;

internal sealed class GetRoleDetailsHandler(IApplicationDbContext db)
    : IRequestHandler<GetRoleDetailsQuery, IReadOnlyList<RoleDetailDto>>
{
    public async Task<IReadOnlyList<RoleDetailDto>> HandleAsync(GetRoleDetailsQuery request, CancellationToken ct)
    {
        var roles = await db.Roles.AsNoTracking().OrderByDescending(r => r.IsSystem).ThenBy(r => r.Name).ToListAsync(ct);

        var permissions = (await db.RolePermissions.AsNoTracking()
                .Select(rp => new { rp.RoleId, rp.Permission.Key })
                .ToListAsync(ct))
            .ToLookup(x => x.RoleId, x => x.Key);

        var userCounts = (await db.UserRoles.AsNoTracking()
                .GroupBy(ur => ur.RoleId)
                .Select(g => new { RoleId = g.Key, Count = g.Count() })
                .ToListAsync(ct))
            .ToDictionary(x => x.RoleId, x => x.Count);

        var limits = (await db.Set<ApprovalLimit>().AsNoTracking().ToListAsync(ct)).ToDictionary(l => l.RoleId);

        return roles.Select(role =>
        {
            limits.TryGetValue(role.Id, out var limit);

            return new RoleDetailDto(
                role.Id,
                role.Name,
                role.Description,
                role.IsSystem,
                userCounts.GetValueOrDefault(role.Id),
                permissions[role.Id].OrderBy(k => k).ToList(),
                new ApprovalLimitDto(limit?.MaxDiscountPercent ?? 0, limit?.MaxDiscountAmount ?? 0, limit?.CanApproveVoid ?? false, limit?.CanApproveReturn ?? false));
        }).ToList();
    }
}

/// <summary>
/// Creates a custom role, or changes the permissions and approval limits of an existing one.
/// The Owner role is locked (it always has everything), system roles keep their name,
/// and nobody can give a permission they do not hold themselves.
/// </summary>
[RequirePermission(Permissions.RoleManage)]
public sealed record SaveRoleCommand(
    Guid? Id,
    string Name,
    string? Description,
    IReadOnlyList<string> PermissionKeys,
    ApprovalLimitDto Limits) : IRequest<Guid>;

internal sealed class SaveRoleValidator : AbstractValidator<SaveRoleCommand>
{
    public SaveRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(300);
        RuleForEach(x => x.PermissionKeys)
            .Must(key => Permissions.All.Any(p => p.Key == key && p.Module != Permissions.PlatformModule))
            .WithMessage("'{PropertyValue}' is not a permission.");
        RuleFor(x => x.Limits).NotNull();
        RuleFor(x => x.Limits.MaxDiscountPercent).InclusiveBetween(0, 100).When(x => x.Limits is not null);
        RuleFor(x => x.Limits.MaxDiscountAmount).InclusiveBetween(0, 1_000_000_000).When(x => x.Limits is not null);
    }
}

internal sealed class SaveRoleHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SaveRoleCommand, Guid>
{
    public async Task<Guid> HandleAsync(SaveRoleCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Roles belong to a business.");

        var keys = request.PermissionKeys.Distinct().ToList();

        var missing = keys.FirstOrDefault(k => !current.HasPermission(k));

        if (missing is not null)
        {
            throw new ForbiddenException($"You cannot give a permission you do not have: {missing}.");
        }

        var name = request.Name.Trim();
        var isNew = request.Id is null;
        Role role;

        if (isNew)
        {
            role = Role.Create(tenantId, name, request.Description, isSystem: false);
            db.Roles.Add(role);
        }
        else
        {
            role = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, ct)
                ?? throw new RequestRejectedException("role_not_found", "The role does not exist.");

            if (RoleRules.IsOwner(role))
            {
                throw new RequestRejectedException("owner_locked", "The Owner role always has every permission and cannot be changed.");
            }

            if (role.IsSystem)
            {
                if (!string.Equals(role.Name, name, StringComparison.Ordinal))
                {
                    throw new RequestRejectedException("system_role", "A system role cannot be renamed.");
                }
            }
            else
            {
                role.Update(name, request.Description);
            }
        }

        if (await db.Roles.AnyAsync(r => r.Name == name && r.Id != role.Id, ct))
        {
            throw new RequestRejectedException("role_exists", "A role with this name already exists.");
        }

        var permissionIds = await db.Permissions
            .Where(p => keys.Contains(p.Key))
            .ToDictionaryAsync(p => p.Key, p => p.Id, ct);

        var existing = isNew
            ? []
            : await db.RolePermissions.Include(rp => rp.Permission).Where(rp => rp.RoleId == role.Id).ToListAsync(ct);

        var before = existing.Select(e => e.Permission.Key).OrderBy(k => k).ToList();

        foreach (var row in existing.Where(e => !keys.Contains(e.Permission.Key)))
        {
            db.RolePermissions.Remove(row);
        }

        foreach (var key in keys.Where(k => permissionIds.ContainsKey(k) && existing.All(e => e.Permission.Key != k)))
        {
            db.RolePermissions.Add(RolePermission.Create(tenantId, role.Id, permissionIds[key]));
        }

        var limit = await db.Set<ApprovalLimit>().FirstOrDefaultAsync(l => l.RoleId == role.Id, ct);

        if (limit is null)
        {
            limit = new ApprovalLimit { TenantId = tenantId, RoleId = role.Id };
            db.Set<ApprovalLimit>().Add(limit);
        }

        limit.MaxDiscountPercent = request.Limits.MaxDiscountPercent;
        limit.MaxDiscountAmount = request.Limits.MaxDiscountAmount;
        limit.CanApproveVoid = request.Limits.CanApproveVoid;
        limit.CanApproveReturn = request.Limits.CanApproveReturn;

        audit.Add(
            isNew ? "role.created" : "role.updated",
            "Role",
            role.Id,
            isNew ? null : new { permissions = before },
            new { name = role.Name, permissions = keys.OrderBy(k => k).ToList(), request.Limits });

        await db.SaveChangesAsync(ct);
        return role.Id;
    }
}

/// <summary>Deletes a custom role that nobody uses.</summary>
[RequirePermission(Permissions.RoleManage)]
public sealed record DeleteRoleCommand(Guid RoleId) : IRequest<bool>;

internal sealed class DeleteRoleHandler(IApplicationDbContext db, AuditTrail audit) : IRequestHandler<DeleteRoleCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteRoleCommand request, CancellationToken ct)
    {
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct)
            ?? throw new RequestRejectedException("role_not_found", "The role does not exist.");

        if (role.IsSystem)
        {
            throw new RequestRejectedException("system_role", "A system role cannot be deleted.");
        }

        if (await db.UserRoles.AnyAsync(ur => ur.RoleId == role.Id, ct))
        {
            throw new RequestRejectedException("role_in_use", "Some users still have this role. Give them another role first.");
        }

        foreach (var row in await db.RolePermissions.Where(rp => rp.RoleId == role.Id).ToListAsync(ct))
        {
            db.RolePermissions.Remove(row);
        }

        foreach (var row in await db.Set<ApprovalLimit>().Where(l => l.RoleId == role.Id).ToListAsync(ct))
        {
            db.Set<ApprovalLimit>().Remove(row);
        }

        db.Roles.Remove(role);
        audit.Add("role.deleted", "Role", role.Id, new { name = role.Name });

        await db.SaveChangesAsync(ct);
        return true;
    }
}
