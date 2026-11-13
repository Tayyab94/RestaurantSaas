using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestaurantSaaS.Application;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Infrastructure;
using RestaurantSaaS.Infrastructure.Persistence;
using RestaurantSaaS.Infrastructure.Services;
using RestaurantSaaS.Shared.UI;
using RestaurantSaaS.Shared.UI.Services.Auth;

namespace RestaurantSaaS.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var settings = DesktopSettings.Load();

        builder.Services.AddSingleton(settings);
        builder.Services.AddSharedUi();
        builder.Services.AddSingleton(new AuthSessionOptions { RestoreSession = settings.RememberSignIn });
        builder.Services.AddScoped<ISecureStore, MauiSecureStore>();

        if (settings.Mode == AppMode.Connected)
        {
            // Only the screens run here; every call goes to the server API.
            builder.Services.AddRemoteClientServices(new Uri(settings.ServerUrl));
        }
        else
        {
            AddStandaloneMode(builder.Services, settings);
        }

        var app = builder.Build();

        if (settings.Mode == AppMode.Standalone)
        {
            // Applies the migrations and creates the permission catalog. Task.Run keeps the UI thread's
            // synchronization context out of the database start-up.
            Task.Run(() => DatabaseInitializer.InitializeAsync(app.Services)).GetAwaiter().GetResult();
        }

        return app;
    }

    // The application runs inside this app, with its own database on this PC (SQL Server or PostgreSQL).
    private static void AddStandaloneMode(IServiceCollection services, DesktopSettings settings)
    {
        var values = new Dictionary<string, string?>
        {
            ["Database:Provider"] = settings.DatabaseProvider,
            ["Database:ConnectionString"] = settings.ConnectionString,
        };
        values["License:ServerMode"] = "Remote";
        values["License:ServerUrl"] = Preferences.Default.Get("license_server_url", string.Empty);
        values["License:TrustedKeys:k1"] = Preferences.Default.Get("license_public_key", string.Empty);
#if DEBUG
        values["License:ServerMode"] = "Embedded";
        values["License:ServerEnabled"] = "true";
        values["License:KeyPepper"] = "dev-only-pepper-change-me";
        values["License:AllowDevSigningKey"] = "true";
        // (existing Verification:LogCodes and Seed lines stay here)
#endif
 
#if DEBUG
        // Development sign-ins, as in the web project. Never compiled into a release build.
        values["Verification:LogCodes"] = "true";
        values["Seed:SuperAdminPassword"] = "ChangeMe!123";
        values["Seed:DemoData"] = "true";
        values["Seed:DemoOwnerPassword"] = "Demo@12345";
#endif


        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();

        services.AddApplication()
            .AddLocalClientServices()
            .AddInfrastructure(configuration);

        services.AddSingleton<IDeploymentInfo>(new DeploymentInfo(DeploymentMode.Desktop));
        services.AddSingleton<ITokenService, OpaqueTokenService>();
        services.AddScoped<AccessorRequestContext>();
        services.AddScoped<ICurrentUser>(provider => provider.GetRequiredService<AccessorRequestContext>());
        services.AddScoped<ITenantProvider>(provider => provider.GetRequiredService<AccessorRequestContext>());
    }
}
