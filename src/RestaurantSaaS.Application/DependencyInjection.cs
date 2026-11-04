using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Services;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application;

public static class DependencyInjection
{
    /// <summary>Registers the dispatcher, behaviors, validators and every request handler.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IRequestContextAccessor, RequestContextAccessor>();
        services.AddScoped<UserSessionBuilder>();
        services.AddScoped<SignInService>();
        services.AddScoped<AuditTrail>();
        services.TryAddScoped<IRequestInfo, NullRequestInfo>();
        services.AddScoped<TenantProvisioner>();
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Order matters: the first registered behavior runs first (authorization, then validation).
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        foreach (var type in assembly.GetTypes().Where(t => t is { IsAbstract: false, IsInterface: false }))
        {
            foreach (var contract in type.GetInterfaces()
                         .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            {
                services.AddScoped(contract, type);
            }
        }

        return services;
    }

    /// <summary>The UI service implementations that call the application in-process (the Web server UI).</summary>
    public static IServiceCollection AddLocalClientServices(this IServiceCollection services)
    {
        services.AddScoped<LocalExecutor>();
        services.AddScoped<IAuthService, LocalAuthService>();
        services.AddScoped<IUserService, LocalUserService>();
        services.AddScoped<IDashboardService, LocalDashboardService>();
        services.AddScoped<ITenantService, LocalTenantService>();
        services.AddScoped<ISetupService, LocalSetupService>();
        services.AddScoped<IOnboardingService, LocalOnboardingService>();
        services.AddScoped<IPreferenceService, LocalPreferenceService>();
        services.AddScoped<IRoleService, LocalRoleService>();
        services.AddScoped<IApprovalService, LocalApprovalService>();
        services.AddScoped<IAuditService, LocalAuditService>();
        return services;
    }
}
