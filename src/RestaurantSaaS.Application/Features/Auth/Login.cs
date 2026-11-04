using FluentValidation;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Application.Features.Auth;

/// <summary>
/// Sign in. Tenant users give their business code; the SuperAdmin leaves it empty.
/// Every failure returns the same message so nobody can find out which usernames exist.
/// </summary>
[AllowAnonymousRequest]
public sealed record LoginCommand(string? BusinessCode, string UserName, string Password) : IRequest<AuthTokensDto>;

internal sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BusinessCode).MaximumLength(30);
    }
}

internal sealed class LoginHandler(SignInService signIn) : IRequestHandler<LoginCommand, AuthTokensDto>
{
    public Task<AuthTokensDto> HandleAsync(LoginCommand request, CancellationToken ct) =>
        signIn.SignInAsync(request.BusinessCode, request.UserName, request.Password, SignInMethod.Password, ct);
}

/// <summary>Quick sign-in for staff on a shared screen: business code, username and a 4 to 6 digit PIN set by a manager.</summary>
[AllowAnonymousRequest]
public sealed record PinLoginCommand(string BusinessCode, string UserName, string Pin) : IRequest<AuthTokensDto>;

internal sealed class PinLoginValidator : AbstractValidator<PinLoginCommand>
{
    public PinLoginValidator()
    {
        RuleFor(x => x.BusinessCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Pin).Matches("^[0-9]{4,6}$").WithMessage("The PIN has 4 to 6 digits.");
    }
}

internal sealed class PinLoginHandler(SignInService signIn) : IRequestHandler<PinLoginCommand, AuthTokensDto>
{
    public Task<AuthTokensDto> HandleAsync(PinLoginCommand request, CancellationToken ct) =>
        signIn.SignInAsync(request.BusinessCode, request.UserName, request.Pin, SignInMethod.Pin, ct);
}
