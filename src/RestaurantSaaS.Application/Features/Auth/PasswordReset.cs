using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Auth;

internal static class PasswordResetSupport
{
    public const int CodeMinutes = 10;
    public const int MaxCodesPerHour = 3;
    public const int MaxWrongAttempts = 5;

    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    public static string TargetOf(Guid userId) => Hash("pwreset:" + userId);

    public static async Task<User?> FindUserAsync(IApplicationDbContext db, string businessCode, string userName, CancellationToken ct)
    {
        var code = businessCode.Trim().ToUpperInvariant();
        var tenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => !t.IsDeleted && t.Code == code, ct);

        if (tenant is null)
        {
            return null;
        }

        var normalized = User.Normalize(userName);

        return await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => !u.IsDeleted && u.TenantId == tenant.Id && u.NormalizedUserName == normalized, ct);
    }
}

/// <summary>
/// "I forgot my password": sends a 6-digit code to the phone or email on file. The answer is always the same,
/// whether or not the account exists, so nobody can use this to find out who has an account.
/// Staff without a phone or email ask their manager for a temporary password instead.
/// </summary>
[AllowAnonymousRequest]
public sealed record RequestPasswordResetCommand(string BusinessCode, string UserName) : IRequest<bool>;

internal sealed class RequestPasswordResetValidator : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetValidator()
    {
        RuleFor(x => x.BusinessCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(64);
    }
}

internal sealed class RequestPasswordResetHandler(
    IApplicationDbContext db,
    IVerificationSender sender,
    IClock clock,
    IRequestInfo requestInfo) : IRequestHandler<RequestPasswordResetCommand, bool>
{
    public async Task<bool> HandleAsync(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var user = await PasswordResetSupport.FindUserAsync(db, request.BusinessCode, request.UserName, ct);

        if (user is null || !user.IsActive || user.IsSuperAdmin || (string.IsNullOrWhiteSpace(user.Phone) && string.IsNullOrWhiteSpace(user.Email)))
        {
            return true;
        }

        var now = clock.UtcNow;
        var target = PasswordResetSupport.TargetOf(user.Id);

        var recent = await db.Set<VerificationCode>().CountAsync(
            c => c.Purpose == VerificationCodePurpose.PasswordReset && c.TargetHash == target && c.CreatedAt > now.AddHours(-1), ct);

        if (recent >= PasswordResetSupport.MaxCodesPerHour)
        {
            return true;
        }

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        db.Set<VerificationCode>().Add(new VerificationCode
        {
            Purpose = VerificationCodePurpose.PasswordReset,
            TargetHash = target,
            CodeHash = PasswordResetSupport.Hash(code + ":" + target),
            ExpiresAt = now.AddMinutes(PasswordResetSupport.CodeMinutes),
            IpAddress = requestInfo.IpAddress,
        });

        await db.SaveChangesAsync(ct);

        await sender.SendAsync(
            user.Phone,
            user.Email,
            $"Your RestoPOS password reset code is {code}. It works for {PasswordResetSupport.CodeMinutes} minutes.",
            ct);

        return true;
    }
}

/// <summary>Sets a new password with the code that was sent. Five wrong tries use the code up.</summary>
[AllowAnonymousRequest]
public sealed record ResetPasswordCommand(string BusinessCode, string UserName, string Code, string NewPassword) : IRequest<bool>;

internal sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.BusinessCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Code).Matches("^[0-9]{6}$").WithMessage("The code has 6 digits.");
        RuleFor(x => x.NewPassword).MustBeStrongPassword();
    }
}

internal sealed class ResetPasswordHandler(IApplicationDbContext db, IPasswordHasher hasher, IClock clock)
    : IRequestHandler<ResetPasswordCommand, bool>
{
    private static RequestRejectedException BadCode() =>
        new("invalid_code", "The code is not correct or has expired. Ask for a new code.");

    public async Task<bool> HandleAsync(ResetPasswordCommand request, CancellationToken ct)
    {
        var now = clock.UtcNow;
        var user = await PasswordResetSupport.FindUserAsync(db, request.BusinessCode, request.UserName, ct);

        if (user is null || !user.IsActive || user.IsSuperAdmin)
        {
            throw BadCode();
        }

        var target = PasswordResetSupport.TargetOf(user.Id);

        var row = await db.Set<VerificationCode>()
            .Where(c => c.Purpose == VerificationCodePurpose.PasswordReset && c.TargetHash == target && c.UsedAt == null && c.ExpiresAt > now)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (row is null || row.AttemptCount >= PasswordResetSupport.MaxWrongAttempts)
        {
            throw BadCode();
        }

        row.AttemptCount++;

        if (row.CodeHash != PasswordResetSupport.Hash(request.Code + ":" + target))
        {
            await db.SaveChangesAsync(ct);
            throw BadCode();
        }

        row.UsedAt = now;
        user.SetPassword(hasher.Hash(request.NewPassword), now);

        // Every other session of this user ends.
        var sessions = await db.RefreshTokens.IgnoreQueryFilters().Where(t => !t.IsDeleted && t.UserId == user.Id).ToListAsync(ct);

        foreach (var session in sessions)
        {
            session.Revoke(now);
        }

        await db.SaveChangesAsync(ct);
        return true;
    }
}
