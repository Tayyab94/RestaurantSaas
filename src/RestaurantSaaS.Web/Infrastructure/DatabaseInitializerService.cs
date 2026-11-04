using RestaurantSaaS.Infrastructure.Persistence;

namespace RestaurantSaaS.Web.Infrastructure;

/// <summary>
/// Applies migrations and seeds the permission catalog, the default SuperAdmin (and the demo business in development)
/// when the host really starts. It does not run when tools such as "dotnet ef" only build the host.
/// </summary>
internal sealed class DatabaseInitializerService(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) =>
        DatabaseInitializer.InitializeAsync(services, cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
