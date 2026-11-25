using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Features.Onboarding;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Settings;

// FR-M05-01: branches (address, phone, opening hours).

[RequirePermission(Permissions.BranchView)]
public sealed record GetBranchDetailsQuery : IRequest<IReadOnlyList<BranchDetailDto>>;

internal sealed class GetBranchDetailsHandler(IApplicationDbContext db)
    : IRequestHandler<GetBranchDetailsQuery, IReadOnlyList<BranchDetailDto>>
{
    public async Task<IReadOnlyList<BranchDetailDto>> HandleAsync(GetBranchDetailsQuery request, CancellationToken ct)
    {
        var branches = await db.Branches.AsNoTracking()
            .OrderByDescending(b => b.IsMain).ThenBy(b => b.Name)
            .ToListAsync(ct);

        var hours = await db.Set<Setting>().AsNoTracking()
            .Where(s => s.Key == SettingKeys.BranchOpeningHours && s.BranchId != null)
            .Select(s => new { s.BranchId, s.Value })
            .ToListAsync(ct);

        var hoursByBranch = hours
            .GroupBy(h => h.BranchId!.Value)
            .ToDictionary(g => g.Key, g => g.First().Value);

        return branches
            .Select(b => new BranchDetailDto(
                b.Id,
                b.Code,
                b.Name,
                b.City,
                b.Address,
                b.Phone,
                b.IsMain,
                hoursByBranch.GetValueOrDefault(b.Id)))
            .ToList();
    }
}

[RequirePermission(Permissions.BranchManage)]
public sealed record SaveBranchCommand(
    Guid? Id,
    string Code,
    string Name,
    string? City,
    string? Address,
    string? Phone,
    string? OpeningHours) : IRequest<Guid>;

internal sealed partial class SaveBranchValidator : AbstractValidator<SaveBranchCommand>
{
    public SaveBranchValidator()
    {
        // The code is only used when a branch is created.
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30)
            .Matches(CodePattern()).WithMessage("Use letters, digits, dash or underscore only.")
            .When(x => x.Id is null);

        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(400);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.OpeningHours).MaximumLength(500);
    }

    [GeneratedRegex("^[A-Za-z0-9_-]+$")]
    private static partial Regex CodePattern();
}

internal sealed class SaveBranchHandler(
    IApplicationDbContext db,
    ICurrentUser current,
    IClock clock,
    SettingStore settings,
    AuditTrail audit) : IRequestHandler<SaveBranchCommand, Guid>
{
    public async Task<Guid> HandleAsync(SaveBranchCommand request, CancellationToken ct)
    {
        var tenantId = current.TenantId
            ?? throw new RequestRejectedException("no_tenant", "Branches belong to a business.");

        Branch branch;
        var isNew = request.Id is null;

        if (isNew)
        {
            var code = request.Code.Trim().ToUpperInvariant();

            if (await db.Branches.AnyAsync(b => b.Code == code, ct))
            {
                throw new RequestRejectedException("branch_exists", "This branch code is already used.");
            }

            branch = Branch.Create(tenantId, code, request.Name, isMain: false);
            db.Branches.Add(branch);
        }
        else
        {
            branch = await db.Branches.FirstOrDefaultAsync(b => b.Id == request.Id, ct)
                ?? throw new RequestRejectedException("branch_not_found", "The branch does not exist.");
        }

        branch.Update(request.Name, request.City, request.Address, request.Phone);

        var hours = string.IsNullOrWhiteSpace(request.OpeningHours) ? null : request.OpeningHours.Trim();
        await settings.SetAsync(tenantId, branch.Id, SettingKeys.BranchGroup, SettingKeys.BranchOpeningHours, hours, "string", ct);

        // FR-M05-07: the step ticks itself once a branch has an address, a phone and opening hours.
        if (!string.IsNullOrWhiteSpace(branch.Address) && !string.IsNullOrWhiteSpace(branch.Phone) && hours is not null)
        {
            await OnboardingTick.DoneAsync(db, tenantId, current.UserId, clock.UtcNow, OnboardingSteps.BranchKey, ct);
        }

        audit.Add(
            isNew ? "branch.created" : "branch.updated",
            "Branch",
            branch.Id,
            newValues: new { branch.Code, branch.Name, branch.City },
            branchId: branch.Id);

        await db.SaveChangesAsync(ct);
        return branch.Id;
    }
}
