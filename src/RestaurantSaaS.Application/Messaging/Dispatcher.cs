using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantSaaS.Application.Messaging;

/// <summary>
/// A small, dependency-free request dispatcher (replaces MediatR, whose newer versions are commercial).
/// Finds the handler, wraps it in the registered pipeline behaviors and runs it.
/// </summary>
internal sealed class Dispatcher(IServiceProvider services) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, object> Wrappers = new();

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapper = (RequestWrapper<TResponse>)Wrappers.GetOrAdd(
            request.GetType(),
            static requestType => Activator.CreateInstance(
                typeof(RequestWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse)))!);

        return wrapper.HandleAsync(request, services, ct);
    }
}

internal abstract class RequestWrapper<TResponse>
{
    public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider services, CancellationToken ct);
}

internal sealed class RequestWrapperImpl<TRequest, TResponse> : RequestWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider services, CancellationToken ct)
    {
        var typedRequest = (TRequest)request;
        var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = services.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse().ToArray();

        RequestHandlerDelegate<TResponse> next = () => handler.HandleAsync(typedRequest, ct);

        foreach (var behavior in behaviors)
        {
            var inner = next;
            next = () => behavior.HandleAsync(typedRequest, inner, ct);
        }

        return next();
    }
}
