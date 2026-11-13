using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Licensing;

internal sealed class LicenseGate(
    IDeploymentInfo deployment, ICurrentUser user, IApplicationDbContext db,
    LicenseChecker checker, LicenseStateCache cache, IClock clock) : ILicenseGate
{
    public async Task<LicenseStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        if (deployment.Mode == DeploymentMode.Cloud)
        {
            return await CloudStatusAsync(ct);
        }

        var cached = cache.Get(clock.UtcNow);
        return cached ?? await checker.CheckAsync(cache.LastResult is null ? LicenseCheckTrigger.Startup : LicenseCheckTrigger.ServerRequest, ct);
    }

    public async Task<LicenseStatusDto> RefreshAsync(LicenseCheckTrigger trigger, CancellationToken ct = default)
    {
        if (deployment.Mode == DeploymentMode.Cloud)
        {
            return await CloudStatusAsync(ct);
        }

        cache.Clear();
        return await checker.CheckAsync(trigger, ct);
    }

    public async Task EnsureCanSellAsync(CancellationToken ct = default)
    {
        var status = await GetStatusAsync(ct);
        if (status.BlocksSelling)
        {
            throw new LicenseException(status.Message);
        }
    }

    private async Task<LicenseStatusDto> CloudStatusAsync(CancellationToken ct)
    {
        if (user.TenantId is null)
        {
            return LicenseStatuses.Simple(LicenseState.NotRequired);
        }

        var now = clock.UtcNow;
        var tenant = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(ct);
        var subscription = await db.Set<Subscription>().AsNoTracking()
            .Where(s => s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.StartDate).FirstOrDefaultAsync(ct);

        if (subscription is not null)
        {
            var expires = subscription.EndDate?.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            var plan = await db.Set<Plan>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == subscription.PlanId, ct);
            var license = subscription.LicenseId is { } licenseId
                ? await db.Set<License>().AsNoTracking().FirstOrDefaultAsync(l => l.Id == licenseId, ct)
                : null;
            var evaluation = LicenseEvaluator.Evaluate(subscription.LicenseType, expires, now);

            return LicenseStatuses.Build(
                evaluation.State, subscription.LicenseType, plan?.Name, tenant?.Name, license?.KeyHint,
                expires, evaluation.DaysLeft, evaluation.GraceDaysLeft, plan?.MaxDevices ?? 0, plan?.MaxBranches ?? 0);
        }

        if (tenant?.TrialEndsAt is { } trialEnds)
        {
            var evaluation = LicenseEvaluator.Evaluate(LicenseType.Trial, trialEnds, now);
            return LicenseStatuses.Build(evaluation.State, LicenseType.Trial, null, tenant.Name, null, trialEnds, evaluation.DaysLeft, null, 0, 0);
        }

        return LicenseStatuses.Simple(LicenseState.Expired);
    }
}

internal sealed class EmbeddedLicenseServerClient(LicenseServerService server) : ILicenseServerClient
{
    public Task<LicenseIssueDto> ActivateAsync(ServerActivateRequest request, CancellationToken ct) =>
        server.ActivateAsync(request, ct);
    // ADD inside EmbeddedLicenseServerClient:
    public Task DeactivateAsync(ServerDeactivateRequest request, CancellationToken ct) =>
        server.DeactivateAsync(request, ct);
    public Task<LicenseIssueDto> StartTrialAsync(ServerTrialRequest request, CancellationToken ct) =>
        server.StartTrialAsync(request, ct);
}
