using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantSaaS.Application.Common.Exceptions;

namespace RestaurantSaaS.Web.Infrastructure;

/// <summary>One place that turns exceptions into ProblemDetails responses with stable error codes.</summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case ValidationException validation:
                problem = new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Please correct the highlighted fields.", Type = "validation" };
                problem.Extensions["errors"] = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            case AppException app:
                problem = new ProblemDetails { Status = app.StatusCode, Title = app.Message, Type = app.Code };
                break;

            default:
                logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
                problem = new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "An unexpected error occurred.", Type = "server_error" };
                break;
        }

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
