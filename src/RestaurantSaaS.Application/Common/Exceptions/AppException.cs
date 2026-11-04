namespace RestaurantSaaS.Application.Common.Exceptions;

/// <summary>An expected failure with a stable code. The web layer maps it to a ProblemDetails response.</summary>
public abstract class AppException(string code, string message, int statusCode) : Exception(message)
{
    public string Code { get; } = code;

    public int StatusCode { get; } = statusCode;
}

/// <summary>Not signed in, or the credentials are wrong (HTTP 401).</summary>
public sealed class UnauthorizedException(string code, string message) : AppException(code, message, 401)
{
}

/// <summary>Signed in but not allowed (HTTP 403).</summary>
public sealed class ForbiddenException(string message) : AppException("forbidden", message, 403)
{
}

/// <summary>The request is well-formed but not allowed (HTTP 400): page too deep, sort not allowed, duplicate name...</summary>
public sealed class RequestRejectedException(string code, string message) : AppException(code, message, 400)
{
}
