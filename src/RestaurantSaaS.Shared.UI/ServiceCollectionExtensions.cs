using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestaurantSaaS.Contracts.Services;
using RestaurantSaaS.Domain.Security;
using RestaurantSaaS.Shared.UI.Services;
using RestaurantSaaS.Shared.UI.Services.Auth;
using RestaurantSaaS.Shared.UI.Services.Remote;

namespace RestaurantSaaS.Shared.UI;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// UI services every host needs: theme, the sign-in session and the authentication state.
    /// The host must also register an ISecureStore (where the refresh token is kept).
    /// </summary>
    public static IServiceCollection AddSharedUi(this IServiceCollection services)
    {
        services.AddScoped<ThemeService>();
        services.TryAddSingleton<AuthSessionOptions>();
        services.AddScoped<AuthSession>();
        services.AddScoped<ISessionAccessor>(provider => provider.GetRequiredService<AuthSession>());

        services.AddAuthorizationCore(options =>
        {
            // One policy per permission, named by its key. A page asks for it with [Authorize(Policy = Permissions.UserView)].
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission.Key, policy => policy.RequireClaim("perm", permission.Key));
            }
        });
        services.AddCascadingAuthenticationState();
        services.Replace(ServiceDescriptor.Scoped<AuthenticationStateProvider, AppAuthenticationStateProvider>());

        return services;
    }

    /// <summary>
    /// Remote implementations of the UI services (HTTP to the server API).
    /// Used by the desktop app, which always talks to the server (on the same PC, on the LAN or in the cloud).
    /// </summary>
    public static IServiceCollection AddRemoteClientServices(this IServiceCollection services, Uri serverBaseAddress)
    {
        services.AddHttpClient<RemoteApiClient>(client => client.BaseAddress = serverBaseAddress);

        services.AddScoped<IAuthService, RemoteAuthService>();
        services.AddScoped<IUserService, RemoteUserService>();
        services.AddScoped<IDashboardService, RemoteDashboardService>();
        services.AddScoped<ITenantService, RemoteTenantService>();
        services.AddScoped<ISetupService, RemoteSetupService>();
        services.AddScoped<IOnboardingService, RemoteOnboardingService>();
        services.AddScoped<IPreferenceService, RemotePreferenceService>();
        services.AddScoped<IRoleService, RemoteRoleService>();
        services.AddScoped<IApprovalService, RemoteApprovalService>();
        services.AddScoped<IAuditService, RemoteAuditService>();

        return services;
    }
}
