using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Roles;

/// <summary>
/// Manager PIN override. A cashier or waiter hits a limit (a bigger discount, a void, a return...) and a manager
/// types their username and PIN on the same screen. The approval is allowed only when the manager's role limits allow it
/// (the Owner has no limit), it is written to the approval log, and wrong PINs count toward the lock-out.
/// Orders, billing and returns call this when they are built.
/// </summary>
public sealed record RequestApprovalCommand(
    string ApprovalType,
    Guid BranchId,
    string ApproverUserName,
    string Pin,
    string ReferenceType,
    Guid ReferenceId,
    decimal? Amount,
    decimal? Percent,
    string? Reason) : IRequest<ApprovalResultDto>;

internal sealed class RequestApprovalValidator : AbstractValidator<RequestApprovalCommand>
{
    public RequestApprovalValidator()
    {
        RuleFor(x => x.ApprovalType).Must(t => Enum.TryParse<ApprovalLogApprovalType>(t, true, out _)).WithMessage("This kind of approval does not exist.");
        RuleFor(x => x.ApproverUserName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Pin).Matches("^[0-9]{4,6}$").WithMessage("The PIN has 4 to 6 digits.");
        RuleFor(x => x.ReferenceType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Percent).InclusiveBetween(0, 100).When(x => x.Percent.HasValue);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).When(x => x.Amount.HasValue);
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}

internal sealed class RequestApprovalHandler(
    IApplicationDbContext db,
    ICurrentUser current,
    IPasswordHasher hasher,
    IClock clock,
    AuditTrail audit) : IRequestHandler<RequestApprovalCommand, ApprovalResultDto>
{
    private static RequestRejectedException Bad() =>
        new("invalid_approval", "The manager or the PIN is not correct.");

    public async Task<ApprovalResultDto> HandleAsync(RequestApprovalCommand request, CancellationToken ct)
    {
        var requesterId = current.UserId ?? throw new UnauthorizedException("unauthenticated", "Please sign in to continue.");

        if (current.TenantId is null)
        {
            throw new RequestRejectedException("no_tenant", "An approval belongs to a business.");
        }

        if (!await db.Branches.AnyAsync(b => b.Id == request.BranchId, ct))
        {
            throw new RequestRejectedException("branch_not_found", "The branch does not exist.");
        }

        var type = Enum.Parse<ApprovalLogApprovalType>(request.ApprovalType, true);
        var now = clock.UtcNow;
        var normalized = User.Normalize(request.ApproverUserName);

        // The tenant filter limits this to the users of the current business.
        var approver = await db.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalized, ct);

        if (approver is null || !approver.IsActive || approver.PinHash is null || approver.IsSuperAdmin)
        {
            hasher.Verify(request.Pin, hasher.DummyHash);
            throw Bad();
        }

        if (approver.IsLockedOut(now))
        {
            throw new RequestRejectedException("approver_locked", "This manager is locked for a few minutes after wrong PINs.");
        }

        if (!hasher.Verify(request.Pin, approver.PinHash))
        {
            approver.RegisterFailedLogin(now, AuthDefaults.MaxFailedAttempts, TimeSpan.FromMinutes(AuthDefaults.LockoutMinutes));
            await db.SaveChangesAsync(ct);
            throw Bad();
        }

        if (approver.Id == requesterId)
        {
            throw new RequestRejectedException("self_approval", "You cannot approve your own request. Ask another manager.");
        }

        approver.ResetFailedLogins();

        var roles = await db.UserRoles
            .Where(ur => ur.UserId == approver.Id)
            .Select(ur => new { ur.RoleId, ur.Role.Name, ur.Role.IsSystem })
            .ToListAsync(ct);

        var isOwner = roles.Any(r => r.IsSystem && r.Name == SystemRoles.Owner);
        var roleIds = roles.Select(r => r.RoleId).ToList();
        var limits = await db.Set<ApprovalLimit>().AsNoTracking().Where(l => roleIds.Contains(l.RoleId)).ToListAsync(ct);

        var maxPercent = isOwner ? 100m : limits.Select(l => l.MaxDiscountPercent).DefaultIfEmpty(0).Max();
        var maxAmount = isOwner ? decimal.MaxValue : limits.Select(l => l.MaxDiscountAmount).DefaultIfEmpty(0).Max();
        var canVoid = isOwner || limits.Any(l => l.CanApproveVoid);
        var canReturn = isOwner || limits.Any(l => l.CanApproveReturn);

        var allowed = type switch
        {
            ApprovalLogApprovalType.Discount =>
                (request.Percent.HasValue || request.Amount.HasValue)
                && (request.Percent is null || request.Percent <= maxPercent)
                && (request.Amount is null || request.Amount <= maxAmount),
            ApprovalLogApprovalType.Void => canVoid,
            ApprovalLogApprovalType.Return => canReturn,

            // Cash out and price changes need a manager who may also approve voids.
            _ => canVoid,
        };

        if (!allowed)
        {
            throw new RequestRejectedException("beyond_limit", $"{approver.FullName} cannot approve this: it is above their limit. Ask a manager with a higher limit.");
        }

        var log = new ApprovalLog
        {
            BranchId = request.BranchId,
            ApprovalType = type,
            ReferenceType = request.ReferenceType.Trim(),
            ReferenceId = request.ReferenceId,
            RequestedBy = requesterId,
            ApprovedBy = approver.Id,
            Method = ApprovalMethod.Pin,
            Reason = request.Reason,
            Amount = request.Amount,
            ApprovedAt = now,
        };

        db.Set<ApprovalLog>().Add(log);

        audit.Add(
            "approval.granted",
            "ApprovalLog",
            log.Id,
            newValues: new { type = type.ToString(), approver = approver.UserName, request.Percent, request.Amount, request.ReferenceType, request.ReferenceId },
            branchId: request.BranchId);

        await db.SaveChangesAsync(ct);
        return new ApprovalResultDto(log.Id, approver.FullName, type.ToString());
    }
}
