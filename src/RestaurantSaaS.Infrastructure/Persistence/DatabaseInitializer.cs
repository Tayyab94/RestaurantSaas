using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Features.Auth;

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
        await DatabaseSeeder.EnsureSuperAdminAsync(db, provider.GetRequiredService<IPasswordHasher>(), seed, logger, ct);

        if (seed.DemoData)
        {
            await DatabaseSeeder.EnsureDemoTenantAsync(db, provider.GetRequiredService<TenantProvisioner>(), seed, logger, ct);
        }
    }
}
