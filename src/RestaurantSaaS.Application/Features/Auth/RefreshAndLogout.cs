using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Application.Features.Auth;

/// <summary>Exchanges a valid refresh token for a new access token. The refresh token itself stays valid until it expires or is revoked.</summary>
[AllowAnonymousRequest]
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshResultDto>;

internal sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator() => RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(200);
}

internal sealed class RefreshTokenHandler(
    IApplicationDbContext db,
    ITokenService tokens,
    UserSessionBuilder sessionBuilder,
    IClock clock) : IRequestHandler<RefreshTokenCommand, RefreshResultDto>
{
    public async Task<RefreshResultDto> HandleAsync(RefreshTokenCommand request, CancellationToken ct)
    {
        var now = clock.UtcNow;
        var hash = RefreshTokenGenerator.Hash(request.RefreshToken);

        var stored = await db.RefreshTokens.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => !t.IsDeleted && t.TokenHash == hash, ct);

        if (stored is null || !stored.IsActive(now))
        {
            throw new UnauthorizedException("invalid_refresh_token", "Your session has expired. Please sign in again.");
        }

        var user = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == stored.UserId, ct);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException("invalid_refresh_token", "Your session has expired. Please sign in again.");
        }

        var tenant = user.TenantId is null
            ? null
            : await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == user.TenantId, ct);

        if (tenant is not null && !tenant.CanSignIn)
        {
            throw new UnauthorizedException("tenant_suspended", "This business account is not active. Please contact support.");
        }

        var profile = await sessionBuilder.BuildAsync(user, tenant, ct);
        var (accessToken, expiresAt) = tokens.CreateAccessToken(profile);

        return new RefreshResultDto(accessToken, expiresAt, profile);
    }
}

/// <summary>Signs out: the refresh token can no longer be used.</summary>
[AllowAnonymousRequest]
public sealed record LogoutCommand(string RefreshToken) : IRequest<bool>;

internal sealed class LogoutHandler(IApplicationDbContext db, IClock clock) : IRequestHandler<LogoutCommand, bool>
{
    public async Task<bool> HandleAsync(LogoutCommand request, CancellationToken ct)
    {
        var hash = RefreshTokenGenerator.Hash(request.RefreshToken);
        var stored = await db.RefreshTokens.IgnoreQueryFilters().FirstOrDefaultAsync(t => !t.IsDeleted && t.TokenHash == hash, ct);

        if (stored is not null)
        {
            stored.Revoke(clock.UtcNow);
            await db.SaveChangesAsync(ct);
        }

        return true;
    }
}
