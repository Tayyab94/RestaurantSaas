using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Onboarding;

public sealed record OnboardingStepDefinition(string Key, string Title, string Description, string? Route, bool IsAutomatic = false);

/// <summary>
/// The checklist every new business sees on its dashboard. A step with a Route has its screen; the others show "Soon"
/// until their module is built (set the Route then). An automatic step completes itself.
/// </summary>
public static class OnboardingSteps
{
    public const string UsersKey = "users";

    public static IReadOnlyList<OnboardingStepDefinition> All { get; } =
    [
        new("business-info", "Business information", "Check your business name, address, currency and tax details.", null),
        new("branch", "Branch details", "Add the address and opening hours of your branch.", null),
        new("tables", "Tables and floors", "Draw your floors and add your tables.", null),
        new("menu", "Menu", "Add your categories, items and prices.", null),
        new(UsersKey, "Staff users", "Add your manager, cashiers and waiters.", "admin/users", IsAutomatic: true),
        new("printers", "Printers", "Connect the receipt and kitchen printers.", null),
        new("backup", "Backup", "Choose where your data is backed up.", null),
    ];
}

[RequirePermission(Permissions.SettingsView)]
public sealed record GetOnboardingChecklistQuery : IRequest<OnboardingChecklistDto>;

internal sealed class GetOnboardingChecklistHandler(IApplicationDbContext db, ICurrentUser user)
    : IRequestHandler<GetOnboardingChecklistQuery, OnboardingChecklistDto>
{
    public async Task<OnboardingChecklistDto> HandleAsync(GetOnboardingChecklistQuery request, CancellationToken ct)
    {
        // The platform administrator has no business, so no checklist.
        if (user.TenantId is null)
        {
            return new OnboardingChecklistDto([], 0, 0);
        }

        var done = (await db.Set<OnboardingProgress>().AsNoTracking()
                .Where(p => p.IsDone)
                .Select(p => p.StepKey)
                .ToListAsync(ct))
            .ToHashSet();

        // The Owner plus at least one more user means the staff has been added.
        var userCount = await db.Users.CountAsync(ct);

        var steps = OnboardingSteps.All
            .Select(d => new OnboardingStepDto(
                d.Key,
                d.Title,
                d.Description,
                d.Route,
                done.Contains(d.Key) || (d.Key == OnboardingSteps.UsersKey && userCount >= 2),
                d.IsAutomatic))
            .ToList();

        return new OnboardingChecklistDto(steps, steps.Count(s => s.IsDone), steps.Count);
    }
}

[RequirePermission(Permissions.SettingsManage)]
public sealed record SetOnboardingStepCommand(string Key, bool IsDone) : IRequest<bool>;

internal sealed class SetOnboardingStepValidator : AbstractValidator<SetOnboardingStepCommand>
{
    public SetOnboardingStepValidator() =>
        RuleFor(x => x.Key)
            .Must(key => OnboardingSteps.All.Any(s => s.Key == key && !s.IsAutomatic))
            .WithMessage("This step cannot be changed by hand.");
}

internal sealed class SetOnboardingStepHandler(IApplicationDbContext db, ICurrentUser user, IClock clock)
    : IRequestHandler<SetOnboardingStepCommand, bool>
{
    public async Task<bool> HandleAsync(SetOnboardingStepCommand request, CancellationToken ct)
    {
        var tenantId = user.TenantId
            ?? throw new RequestRejectedException("no_tenant", "The checklist belongs to a business.");

        var row = await db.Set<OnboardingProgress>().FirstOrDefaultAsync(p => p.StepKey == request.Key, ct);

        if (row is null)
        {
            row = new OnboardingProgress { TenantId = tenantId, StepKey = request.Key };
            db.Set<OnboardingProgress>().Add(row);
        }

        row.IsDone = request.IsDone;
        row.DoneAt = request.IsDone ? clock.UtcNow : null;
        row.DoneBy = request.IsDone ? user.UserId : null;

        await db.SaveChangesAsync(ct);
        return true;
    }
}
