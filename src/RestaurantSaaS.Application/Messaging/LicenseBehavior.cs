//using RestaurantSaaS.Application.Abstractions;
//using RestaurantSaaS.Application.Security;

//namespace RestaurantSaaS.Application.Messaging;

//internal sealed class LicenseBehavior<TRequest, TResponse>(ILicenseGate gate) : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//{
//    private static readonly bool Required = typeof(TRequest).IsDefined(typeof(RequireActiveLicenseAttribute), inherit: true);

//    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
//    {
//        if (Required)
//        {
//            await gate.EnsureCanSellAsync(ct);
//        }

//        return await next();
//    }
//}


using Microsoft.Extensions.DependencyInjection;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Messaging;

/// Two rules, both about the license:
/// 1. A request marked [RequireActiveLicense] (order, payment) stops when the license
///    does not allow selling.
/// 2. On a desktop or on-premise installation that is NOT activated (never activated, or
///    deactivated by the owner), every request that is not anonymous stops. Only sign-in,
///    sign-out, refresh and the license functions still work, so the system cannot be used
///    until a license is activated again.
/// The gate is resolved only when needed, so a cloud request without the attribute costs nothing.
internal sealed class LicenseBehavior<TRequest, TResponse>(IServiceProvider services) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly bool Required = typeof(TRequest).IsDefined(typeof(RequireActiveLicenseAttribute), inherit: true);
    private static readonly bool Anonymous = typeof(TRequest).IsDefined(typeof(AllowAnonymousRequestAttribute), inherit: true);

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (Required)
        {
            await services.GetRequiredService<ILicenseGate>().EnsureCanSellAsync(ct);
        }
        else if (!Anonymous && services.GetService<IDeploymentInfo>() is { Mode: not DeploymentMode.Cloud })
        {
            var status = await services.GetRequiredService<ILicenseGate>().GetStatusAsync(ct);
            if (status.State == LicenseState.NotActivated)
            {
                throw new LicenseException("This installation is not activated. Activate a license key to use the system.");
            }
        }

        return await next();
    }
}
