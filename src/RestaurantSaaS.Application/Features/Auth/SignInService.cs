using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Application.Features.Auth;

internal enum SignInMethod
{
    Password,
    Pin,
}

/// <summary>
/// The one place that signs a user in, with a password or with a PIN. It applies the same rules to both:
/// one error message for every failure, a constant-time check even for unknown users, lock-out after
/// repeated failures, suspended businesses, a refresh token, and a line in the login history for every attempt.
/// </summary>
internal sealed class SignInService(
    IApplicationDbContext db,
    IPasswordHasher hasher,
    ITokenService tokens,
    UserSessionBuilder sessionBuilder,
    IClock clock,
    IRequestInfo requestInfo)
{
    private static UnauthorizedException Invalid(SignInMethod method) =>
        new("invalid_credentials", method == SignInMethod.Pin
            ? "The business code, username or PIN is not correct."
            : "The business code, username or password is not correct.");

    public async Task<AuthTokensDto> SignInAsync(string? businessCode, string userName, string secret, SignInMethod method, CancellationToken ct)
    {
        var now = clock.UtcNow;
        Tenant? tenant = null;

        if (!string.IsNullOrWhiteSpace(businessCode))
        {
            var code = businessCode.Trim().ToUpperInvariant();
            tenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => !t.IsDeleted && t.Code == code, ct);

            if (tenant is null)
            {
                throw await FailAsync(null, null, userName, "unknown_business", method, secret, ct);
            }
        }
        else if (method == SignInMethod.Pin)
        {
            // A PIN is always used inside a business; the platform administrator signs in with a password.
            throw await FailAsync(null, null, userName, "no_business", method, secret, ct);
        }

        var tenantId = tenant?.Id;
        var normalized = User.Normalize(userName);

        var user = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => !u.IsDeleted && u.TenantId == tenantId && u.NormalizedUserName == normalized, ct);

        if (user is null)
        {
            throw await FailAsync(tenantId, null, userName, "unknown_user", method, secret, ct);
        }

        if (!user.IsActive)
        {
            throw await FailAsync(tenantId, user.Id, userName, "disabled", method, secret, ct);
        }

        if (method == SignInMethod.Pin && (user.IsSuperAdmin || user.PinHash is null))
        {
            throw await FailAsync(tenantId, user.Id, userName, "no_pin", method, secret, ct);
        }

        if (user.IsLockedOut(now))
        {
            Record(tenantId, user.Id, userName, false, "locked");
            await db.SaveChangesAsync(ct);
            throw new UnauthorizedException("locked", "Too many wrong attempts. Please try again in a few minutes.");
        }

        var hash = method == SignInMethod.Pin ? user.PinHash! : user.PasswordHash;

        if (!hasher.Verify(secret, hash))
        {
            user.RegisterFailedLogin(now, AuthDefaults.MaxFailedAttempts, TimeSpan.FromMinutes(AuthDefaults.LockoutMinutes));
            Record(tenantId, user.Id, userName, false, method == SignInMethod.Pin ? "wrong_pin" : "wrong_password");
            await db.SaveChangesAsync(ct);
            throw Invalid(method);
        }

        if (tenant is not null && !tenant.CanSignIn)
        {
            Record(tenantId, user.Id, userName, false, "tenant_suspended");
            await db.SaveChangesAsync(ct);
            throw new UnauthorizedException("tenant_suspended", "This business account is not active. Please contact support.");
        }

        user.RegisterSuccessfulLogin(now);

        var (refreshToken, refreshHash) = RefreshTokenGenerator.Generate();
        db.RefreshTokens.Add(RefreshToken.Create(user.Id, refreshHash, now.AddDays(AuthDefaults.RefreshTokenDays)));
        Record(tenantId, user.Id, userName, true, null);
        await db.SaveChangesAsync(ct);

        var profile = await sessionBuilder.BuildAsync(user, tenant, ct);
        var (accessToken, expiresAt) = tokens.CreateAccessToken(profile);

        return new AuthTokensDto(accessToken, expiresAt, refreshToken, profile);
    }

    private async Task<Exception> FailAsync(Guid? tenantId, Guid? userId, string userName, string reason, SignInMethod method, string secret, CancellationToken ct)
    {
        // Same time whether or not the account exists.
        hasher.Verify(secret, hasher.DummyHash);

        Record(tenantId, userId, userName, false, reason);
        await db.SaveChangesAsync(ct);

        return Invalid(method);
    }

    private void Record(Guid? tenantId, Guid? userId, string userName, bool success, string? failureReason) =>
        db.Set<LoginHistory>().Add(new LoginHistory
        {
            TenantId = tenantId,
            UserId = userId,
            UserNameTried = userName.Length > 200 ? userName[..200] : userName,
            IsSuccess = success,
            FailureReason = failureReason,
            IpAddress = requestInfo.IpAddress,
            AttemptedAt = clock.UtcNow,
        });
}
