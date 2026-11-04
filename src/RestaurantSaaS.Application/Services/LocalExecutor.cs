using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

/// <summary>
/// Runs a request in-process for the UI (Web server). It opens a fresh DI scope per call (a Blazor circuit lives long;
/// a long-lived DbContext would go stale), puts the signed-in user of the UI session into that scope,
/// and turns application errors into the UI's ServiceException.
/// </summary>
internal sealed class LocalExecutor(IServiceScopeFactory scopes, IServiceProvider services)
{
    public async Task<T> SendAsync<T>(IRequest<T> request, CancellationToken ct)
    {
        await using var scope = scopes.CreateAsyncScope();

        // Resolved late on purpose: the session depends on the auth service, which depends on this class.
        var user = services.GetService<ISessionAccessor>()?.User;
        var accessor = scope.ServiceProvider.GetRequiredService<IRequestContextAccessor>();

        accessor.Current = user is null
            ? null
            : new RequestContext(user.Id, user.UserName, user.TenantId, user.IsSuperAdmin, user.Permissions.ToHashSet());

        try
        {
            return await scope.ServiceProvider.GetRequiredService<IDispatcher>().SendAsync(request, ct);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new ServiceException("validation", "Please correct the highlighted fields.", errors);
        }
        catch (AppException ex)
        {
            throw new ServiceException(ex.Code, ex.Message);
        }
    }
}
