namespace RestaurantSaaS.Application.Messaging;

/// <summary>Marker for a command or a query that returns <typeparamref name="TResponse"/>.</summary>
public interface IRequest<out TResponse>
{
}

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>Cross-cutting step around every handler (validation, transaction, logging...).</summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct);
}

public interface IDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
}
