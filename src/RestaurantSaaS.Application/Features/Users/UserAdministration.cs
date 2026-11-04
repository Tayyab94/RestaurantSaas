using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Common.Paging;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Users;

internal static class UserAdministrationSupport
{
    public static async Task<User> FindUserAsync(IApplicationDbContext db, Guid userId, CancellationToken ct) =>
        // The tenant filter limits this to the users of the current business.
        await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
        ?? throw new RequestRejectedException("user_not_found", "The user does not exist.");

    public static async Task RevokeSessionsAsync(IApplicationDbContext db, Guid userId, DateTime now, CancellationToken ct)
    {
        var sessions = await db.RefreshTokens.IgnoreQueryFilters().Where(t => !t.IsDeleted && t.UserId == userId).ToListAsync(ct);

        foreach (var session in sessions)
        {
            session.Revoke(now);
        }
    }

    /// <summary>True when the user holds the Owner role and no other active Owner exists.</summary>
    public static async Task<bool> IsLastActiveOwnerAsync(IApplicationDbContext db, Guid userId, CancellationToken ct)
    {
        var isOwner = await db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.Role.Name == SystemRoles.Owner, ct);

        if (!isOwner)
        {
            return false;
        }

        var activeOwners = await db.UserRoles.CountAsync(ur => ur.Role.Name == SystemRoles.Owner && ur.User.IsActive, ct);
        return activeOwners <= 1;
    }
}

/// <summary>A manager gives a user a 4 to 6 digit PIN for quick sign-in on a shared screen, or removes it (null or empty PIN).</summary>
[RequirePermission(Permissions.UserManage)]
public sealed record SetUserPinCommand(Guid UserId, string? Pin) : IRequest<bool>;

internal sealed class SetUserPinValidator : AbstractValidator<SetUserPinCommand>
{
    public SetUserPinValidator() =>
        RuleFor(x => x.Pin)
            .Matches("^[0-9]{4,6}$").WithMessage("The PIN has 4 to 6 digits.")
            .Must(pin => !IsWeak(pin!)).WithMessage("This PIN is too easy to guess. Do not use repeated or running digits.")
            .When(x => !string.IsNullOrEmpty(x.Pin));

    private static bool IsWeak(string pin) =>
        pin.Distinct().Count() == 1 || "0123456789".Contains(pin) || "9876543210".Contains(pin);
}

internal sealed class SetUserPinHandler(IApplicationDbContext db, IPasswordHasher hasher, IClock clock, AuditTrail audit)
    : IRequestHandler<SetUserPinCommand, bool>
{
    public async Task<bool> HandleAsync(SetUserPinCommand request, CancellationToken ct)
    {
        var user = await UserAdministrationSupport.FindUserAsync(db, request.UserId, ct);

        if (user.IsSuperAdmin)
        {
            throw new RequestRejectedException("not_allowed", "A PIN cannot be set for the platform administrator.");
        }

        if (string.IsNullOrEmpty(request.Pin))
        {
            user.ClearPin();
        }
        else
        {
            user.SetPin(hasher.Hash(request.Pin), clock.UtcNow);
        }

        audit.Add(string.IsNullOrEmpty(request.Pin) ? "user.pin.removed" : "user.pin.set", "User", user.Id, newValues: new { user.UserName });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>A manager gives a user a temporary password (for a user who forgot it and has no phone or email on file).</summary>
[RequirePermission(Permissions.UserManage)]
public sealed record ResetUserPasswordCommand(Guid UserId, string NewPassword) : IRequest<bool>;

internal sealed class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordValidator() => RuleFor(x => x.NewPassword).MustBeStrongPassword();
}

internal sealed class ResetUserPasswordHandler(IApplicationDbContext db, ICurrentUser current, IPasswordHasher hasher, IClock clock, AuditTrail audit)
    : IRequestHandler<ResetUserPasswordCommand, bool>
{
    public async Task<bool> HandleAsync(ResetUserPasswordCommand request, CancellationToken ct)
    {
        if (request.UserId == current.UserId)
        {
            throw new RequestRejectedException("not_allowed", "Use Change password for your own account.");
        }

        var user = await UserAdministrationSupport.FindUserAsync(db, request.UserId, ct);

        if (user.IsSuperAdmin)
        {
            throw new RequestRejectedException("not_allowed", "The platform administrator's password cannot be reset here.");
        }

        var now = clock.UtcNow;
        user.SetPassword(hasher.Hash(request.NewPassword), now, mustChangePassword: true);
        await UserAdministrationSupport.RevokeSessionsAsync(db, user.Id, now, ct);

        audit.Add("user.password.reset", "User", user.Id, newValues: new { user.UserName });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Disables or enables a user. A disabled user cannot sign in and all sessions end. The last active Owner cannot be disabled.</summary>
[RequirePermission(Permissions.UserManage)]
public sealed record SetUserActiveCommand(Guid UserId, bool IsActive) : IRequest<bool>;

internal sealed class SetUserActiveHandler(IApplicationDbContext db, ICurrentUser current, IClock clock, AuditTrail audit)
    : IRequestHandler<SetUserActiveCommand, bool>
{
    public async Task<bool> HandleAsync(SetUserActiveCommand request, CancellationToken ct)
    {
        if (request.UserId == current.UserId)
        {
            throw new RequestRejectedException("not_allowed", "You cannot disable your own account.");
        }

        var user = await UserAdministrationSupport.FindUserAsync(db, request.UserId, ct);

        if (request.IsActive)
        {
            user.Activate();
        }
        else
        {
            if (await UserAdministrationSupport.IsLastActiveOwnerAsync(db, user.Id, ct))
            {
                throw new RequestRejectedException("last_owner", "The last active Owner cannot be disabled.");
            }

            user.Deactivate();
            await UserAdministrationSupport.RevokeSessionsAsync(db, user.Id, clock.UtcNow, ct);
        }

        audit.Add(request.IsActive ? "user.enabled" : "user.disabled", "User", user.Id, newValues: new { user.UserName });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Gives a user another role. A user has one role. The last active Owner cannot lose the Owner role.</summary>
[RequirePermission(Permissions.UserManage)]
public sealed record SetUserRoleCommand(Guid UserId, Guid RoleId) : IRequest<bool>;

internal sealed class SetUserRoleHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SetUserRoleCommand, bool>
{
    public async Task<bool> HandleAsync(SetUserRoleCommand request, CancellationToken ct)
    {
        if (request.UserId == current.UserId)
        {
            throw new RequestRejectedException("not_allowed", "You cannot change your own role.");
        }

        var tenantId = current.TenantId ?? throw new RequestRejectedException("no_tenant", "Roles belong to a business.");
        var user = await UserAdministrationSupport.FindUserAsync(db, request.UserId, ct);

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct)
            ?? throw new RequestRejectedException("role_not_found", "The selected role does not exist.");

        var existing = await db.UserRoles.Include(ur => ur.Role).Where(ur => ur.UserId == user.Id).ToListAsync(ct);

        if (existing.Any(ur => ur.RoleId == role.Id))
        {
            return true;
        }

        var losesOwner = existing.Any(ur => ur.Role.IsSystem && ur.Role.Name == SystemRoles.Owner);

        if (losesOwner && await UserAdministrationSupport.IsLastActiveOwnerAsync(db, user.Id, ct))
        {
            throw new RequestRejectedException("last_owner", "The last active Owner must keep the Owner role.");
        }

        // Giving the Owner role needs the Owner's own authority: everything the Owner has.
        if (role.IsSystem && role.Name == SystemRoles.Owner && !current.HasPermission(Permissions.RoleManage))
        {
            throw new ForbiddenException("Only a user who can manage roles can make someone an Owner.");
        }

        foreach (var old in existing)
        {
            db.UserRoles.Remove(old);
        }

        db.UserRoles.Add(UserRole.Create(tenantId, user.Id, role.Id));

        audit.Add("user.role.changed", "User", user.Id, new { roles = existing.Select(e => e.Role.Name).ToList() }, new { user.UserName, role = role.Name });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>Sets the branches a user may work in. The first one is the default. (The Owner always has all branches.)</summary>
[RequirePermission(Permissions.UserManage)]
public sealed record SetUserBranchesCommand(Guid UserId, IReadOnlyList<Guid> BranchIds) : IRequest<bool>;

internal sealed class SetUserBranchesHandler(IApplicationDbContext db, ICurrentUser current, AuditTrail audit)
    : IRequestHandler<SetUserBranchesCommand, bool>
{
    public async Task<bool> HandleAsync(SetUserBranchesCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId ?? throw new RequestRejectedException("no_tenant", "Branches belong to a business.");
        var user = await UserAdministrationSupport.FindUserAsync(db, request.UserId, ct);

        var branchIds = request.BranchIds.Distinct().ToList();
        var existingBranches = await db.Branches.Select(b => b.Id).ToListAsync(ct);

        if (branchIds.Count == 0 || branchIds.Any(id => !existingBranches.Contains(id)))
        {
            throw new RequestRejectedException("branch_required", "Choose at least one branch.");
        }

        var current_ = await db.Set<UserBranch>().Where(ub => ub.UserId == user.Id).ToListAsync(ct);

        foreach (var row in current_)
        {
            db.Set<UserBranch>().Remove(row);
        }

        foreach (var branchId in branchIds)
        {
            db.Set<UserBranch>().Add(new UserBranch { TenantId = tenantId, UserId = user.Id, BranchId = branchId, IsDefault = branchId == branchIds[0] });
        }

        audit.Add("user.branches.changed", "User", user.Id, new { branches = current_.Select(c => c.BranchId).ToList() }, new { user.UserName, branches = branchIds });

        await db.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>The branches of the business (for the screens that give users their branches).</summary>
[RequirePermission(Permissions.UserView)]
public sealed record GetBranchesQuery : IRequest<IReadOnlyList<BranchDto>>;

internal sealed class GetBranchesHandler(IApplicationDbContext db) : IRequestHandler<GetBranchesQuery, IReadOnlyList<BranchDto>>
{
    public async Task<IReadOnlyList<BranchDto>> HandleAsync(GetBranchesQuery request, CancellationToken ct)
    {
        var branches = await db.Branches.AsNoTracking().OrderByDescending(b => b.IsMain).ThenBy(b => b.Name).ToListAsync(ct);
        return branches.Select(b => new BranchDto(b.Id, b.Code, b.Name, b.IsMain)).ToList();
    }
}

/// <summary>Every sign-in attempt of this business (or, for the platform administrator, of the platform), newest first.</summary>
[RequirePermission(Permissions.UserView)]
public sealed record GetLoginHistoryPageQuery(PageRequest Page) : IRequest<PagedResult<LoginHistoryItemDto>>;

internal sealed class GetLoginHistoryPageHandler(IApplicationDbContext db)
    : IRequestHandler<GetLoginHistoryPageQuery, PagedResult<LoginHistoryItemDto>>
{
    public async Task<PagedResult<LoginHistoryItemDto>> HandleAsync(GetLoginHistoryPageQuery request, CancellationToken ct)
    {
        IQueryable<LoginHistory> query = db.Set<LoginHistory>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
        {
            var term = request.Page.Search.Trim();
            query = query.Where(h => h.UserNameTried.Contains(term));
        }

        return await query
            .OrderByDescending(h => h.AttemptedAt).ThenByDescending(h => h.Id)
            .Select(h => new LoginHistoryItemDto(h.Id, h.UserNameTried, h.IsSuccess, h.FailureReason, h.IpAddress, h.AttemptedAt))
            .ToPagedResultAsync(request.Page, ct);
    }
}
