using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Infrastructure.Persistence;

namespace RestaurantSaaS.Web.Infrastructure;

internal sealed class LicenseMaintenanceService(IServiceProvider services, IDeploymentInfo deployment, ILogger<LicenseMaintenanceService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "The license maintenance job failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        await using var scope = services.CreateAsyncScope();

        if (deployment.Mode != DeploymentMode.Cloud)
        {
            await scope.ServiceProvider.GetRequiredService<ILicenseGate>().RefreshAsync(LicenseCheckTrigger.Periodic, ct);
            return;
        }

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        var now = clock.UtcNow;
        var today = DateOnly.FromDateTime(now);

        var subscriptions = await db.Set<Subscription>().IgnoreQueryFilters()
            .Where(s => !s.IsDeleted && s.Status == SubscriptionStatus.Active && s.EndDate != null && s.EndDate < today)
            .ToListAsync(ct);

        foreach (var subscription in subscriptions)
        {
            subscription.Status = SubscriptionStatus.Expired;
        }

        var licenses = await db.Set<License>()
            .Where(l => l.Status == LicenseStatus.Active && l.ExpiresAt != null && l.ExpiresAt < now)
            .ToListAsync(ct);

        foreach (var license in licenses)
        {
            license.Status = LicenseStatus.Expired;
            db.Set<LicenseEvent>().Add(new LicenseEvent
            {
                LicenseId = license.Id,
                TenantId = license.TenantId,
                EventType = LicenseEventType.Expired,
                Result = "ok",
                OccurredAt = now,
            });
        }

        if (subscriptions.Count + licenses.Count > 0)
        {
            await db.SaveChangesAsync(ct);
            logger.LogInformation("License job: {Subscriptions} subscriptions and {Licenses} licenses expired.", subscriptions.Count, licenses.Count);
        }
    }
}
