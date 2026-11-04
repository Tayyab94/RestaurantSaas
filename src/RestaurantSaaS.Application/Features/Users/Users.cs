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

/// <summary>Offset-paged list of the users of the current business (platform users for the SuperAdmin).</summary>
[RequirePermission(Permissions.UserView)]
public sealed record GetUsersPageQuery(PageRequest Page) : IRequest<PagedResult<UserListItemDto>>;

internal sealed class GetUsersPageHandler(IApplicationDbContext db)
    : IRequestHandler<GetUsersPageQuery, PagedResult<UserListItemDto>>
{
    public async Task<PagedResult<UserListItemDto>> HandleAsync(GetUsersPageQuery request, CancellationToken ct)
    {
        var page = request.Page;
        IQueryable<User> query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var term = page.Search.Trim();
            query = query.Where(u => u.UserName.Contains(term) || u.FullName.Contains(term));
        }

        query = (page.Sort?.ToLowerInvariant(), page.Desc) switch
        {
            (null, _) => query.OrderBy(u => u.FullName).ThenBy(u => u.Id),
            ("fullname", false) => query.OrderBy(u => u.FullName).ThenBy(u => u.Id),
            ("fullname", true) => query.OrderByDescending(u => u.FullName).ThenByDescending(u => u.Id),
            ("username", false) => query.OrderBy(u => u.UserName).ThenBy(u => u.Id),
            ("username", true) => query.OrderByDescending(u => u.UserName).ThenByDescending(u => u.Id),
            _ => throw new RequestRejectedException("invalid_sort", $"Sorting by '{page.Sort}' is not allowed."),
        };

        var rows = await query
            .Select(u => new Row(u.Id, u.UserName, u.FullName, u.Email, u.IsActive, u.LastLoginAt, u.UserRoles.Select(ur => ur.Role.Name).ToList(), u.PinHash != null,
                db.Set<UserBranch>().Where(ub => ub.UserId == u.Id).Select(ub => new BranchRow(ub.BranchId, ub.Branch.Name)).ToList(),
                u.UserRoles.Select(ur => (Guid?)ur.RoleId).FirstOrDefault()))
            .ToPagedResultAsync(page, ct);

        // The Owner may work in every branch.
        var allBranches = await db.Branches.AsNoTracking().OrderBy(b => b.Name).Select(b => new BranchRow(b.Id, b.Name)).ToListAsync(ct);

        var items = rows.Items
            .Select(r => r.Roles.Contains(SystemRoles.Owner)
                ? new UserListItemDto(r.Id, r.UserName, r.FullName, r.Email, r.IsActive, r.LastLoginAt, r.Roles, r.HasPin,
                    ["All branches"], allBranches.Select(b => b.Id).ToList(), r.RoleId)
                : new UserListItemDto(r.Id, r.UserName, r.FullName, r.Email, r.IsActive, r.LastLoginAt, r.Roles, r.HasPin,
                    r.Branches.Select(b => b.Name).ToList(), r.Branches.Select(b => b.Id).ToList(), r.RoleId))
            .ToList();

        return new PagedResult<UserListItemDto>(items, rows.Page, rows.PageSize, rows.TotalCount);
    }

    private sealed record Row(Guid Id, string UserName, string FullName, string? Email, bool IsActive, DateTime? LastLoginAt, List<string> Roles, bool HasPin, List<BranchRow> Branches, Guid? RoleId);

    private sealed record BranchRow(Guid Id, string Name);
}

[RequirePermission(Permissions.UserView)]
public sealed record GetRolesQuery : IRequest<IReadOnlyList<RoleDto>>;

internal sealed class GetRolesHandler(IApplicationDbContext db) : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleDto>>
{
    public async Task<IReadOnlyList<RoleDto>> HandleAsync(GetRolesQuery request, CancellationToken ct)
    {
        var roles = await db.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);
        return roles.Select(r => new RoleDto(r.Id, r.Name, r.Description, r.IsSystem)).ToList();
    }
}

[RequirePermission(Permissions.UserManage)]
public sealed record CreateUserCommand(
    string UserName,
    string FullName,
    string? Email,
    string? Phone,
    string Password,
    Guid RoleId,
    IReadOnlyList<Guid> BranchIds) : IRequest<Guid>;

internal sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(50);
           // .Matches("^[A-Za-z0-9._-]+$").WithMessage("Use letters, digits, dot, dash or underscore only.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Password).MustBeStrongPassword();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

internal sealed class CreateUserHandler(IApplicationDbContext db, ICurrentUser current, IPasswordHasher hasher, AuditTrail audit)
    : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateUserCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Users can only be created inside a business.");

        var normalized = User.Normalize(request.UserName);

        // The tenant filter limits both queries to the current business.
        if (await db.Users.AnyAsync(u => u.NormalizedUserName == normalized, ct))
        {
            throw new RequestRejectedException("user_exists", "This username is already used in your business.");
        }

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct)
            ?? throw new RequestRejectedException("role_not_found", "The selected role does not exist.");

        var user = User.Create(tenantId, request.UserName, request.FullName, hasher.Hash(request.Password), email: request.Email, phone: request.Phone);

        db.Users.Add(user);
        db.UserRoles.Add(UserRole.Create(tenantId, user.Id, role.Id));

        // A user needs at least one branch (the Owner works in all of them). With a single branch it is chosen for you.
        var branchIds = request.BranchIds.Distinct().ToList();
        var existingBranches = await db.Branches.Select(b => b.Id).ToListAsync(ct);

        if (branchIds.Count == 0 && existingBranches.Count == 1)
        {
            branchIds = existingBranches;
        }

        if (branchIds.Count == 0 || branchIds.Any(id => !existingBranches.Contains(id)))
        {
            throw new RequestRejectedException("branch_required", "Choose at least one branch.");
        }

        foreach (var branchId in branchIds)
        {
            db.Set<UserBranch>().Add(new UserBranch { TenantId = tenantId, UserId = user.Id, BranchId = branchId, IsDefault = branchId == branchIds[0] });
        }

        audit.Add("user.created", "User", user.Id, newValues: new { user.UserName, role = role.Name, branches = branchIds });

        await db.SaveChangesAsync(ct);

        return user.Id;
    }
}
