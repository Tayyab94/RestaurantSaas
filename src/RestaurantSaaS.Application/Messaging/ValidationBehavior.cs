using FluentValidation;

namespace RestaurantSaaS.Application.Messaging;

/// <summary>Runs every FluentValidation validator of the request before the handler. Failures throw ValidationException (HTTP 400).</summary>
internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var validatorList = validators.ToList();

        if (validatorList.Count > 0)
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(validatorList.Select(v => v.ValidateAsync(context, ct)));
            var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
