using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Application.Features.Settings;

/// <summary>Ticks an automatic onboarding step. It only changes the row; the calling handler saves it.</summary>
internal static class OnboardingTick
{
    public static async Task DoneAsync(IApplicationDbContext db, Guid tenantId, Guid? userId, DateTime utcNow, string key, CancellationToken ct)
    {
        var row = await db.Set<OnboardingProgress>().FirstOrDefaultAsync(p => p.StepKey == key, ct);

        if (row is null)
        {
            row = new OnboardingProgress { TenantId = tenantId, StepKey = key };
            db.Set<OnboardingProgress>().Add(row);
        }

        if (row.IsDone)
        {
            return;
        }

        row.IsDone = true;
        row.DoneAt = utcNow;
        row.DoneBy = userId;
    }
}
