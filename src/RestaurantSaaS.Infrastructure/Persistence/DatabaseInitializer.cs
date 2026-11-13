using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    /// <summary>
    /// Applies the EF Core migrations of the configured provider, then makes sure the permission catalog,
    /// the default SuperAdmin (and, in development, the demo business) exist.
    /// Create the first migration with scripts/add-migration.ps1.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<AppDbContext>();
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
        var seed = provider.GetRequiredService<IOptions<SeedOptions>>().Value;

        if (!db.Database.GetMigrations().Any())
        {
            logger.LogWarning("No EF Core migrations were found for this provider. Create the first one with: scripts/add-migration.ps1 -Provider <SqlServer|PostgreSql> -Name Initial");
            return;
        }

        await db.Database.MigrateAsync(ct);
        await DatabaseSeeder.EnsurePermissionsAsync(db, ct);
        await DatabaseSeeder.EnsureOwnerPermissionsAsync(db, ct);
       

        // ADD after DatabaseSeeder.EnsureOwnerPermissionsAsync(db, ct);
        await DatabaseSeeder.EnsureDefaultPlanAsync(db, ct);

        // ADD after the "if (seed.DemoData) { ... }" block:
        if (provider.GetRequiredService<IDeploymentInfo>().Mode != DeploymentMode.Cloud)
        {
            try
            {
                var status = await provider.GetRequiredService<ILicenseGate>().RefreshAsync(LicenseCheckTrigger.Startup, ct);
                logger.LogInformation("License check at start-up: {State}.", status.State);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "The license check at start-up failed.");
            }
        }
        await DatabaseSeeder.EnsureSuperAdminAsync(db, provider.GetRequiredService<IPasswordHasher>(), seed, logger, ct);

        if (seed.DemoData)
        {
            await DatabaseSeeder.EnsureDemoTenantAsync(db, provider.GetRequiredService<TenantProvisioner>(), seed, logger, ct);
        }
    }
}
