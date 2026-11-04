using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;

namespace RestaurantSaaS.Application.Features.Auth;

/// <summary>Changes the signed-in user's password and signs out every other session.</summary>
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<bool>;

internal sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).MustBeStrongPassword();
        RuleFor(x => x.NewPassword).NotEqual(x => x.CurrentPassword).WithMessage("The new password must be different.");
    }
}

internal sealed class ChangePasswordHandler(
    IApplicationDbContext db,
    ICurrentUser current,
    IPasswordHasher hasher,
    IClock clock) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> HandleAsync(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = current.UserId ?? throw new UnauthorizedException("unauthenticated", "Please sign in to continue.");
        var now = clock.UtcNow;

        var user = await db.Users.IgnoreQueryFilters().FirstAsync(u => !u.IsDeleted && u.Id == userId, ct);

        if (!hasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new RequestRejectedException("wrong_password", "The current password is not correct.");
        }

        user.SetPassword(hasher.Hash(request.NewPassword), now);

        var tokens = await db.RefreshTokens.IgnoreQueryFilters().Where(t => !t.IsDeleted && t.UserId == userId).ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.Revoke(now);
        }

        await db.SaveChangesAsync(ct);
        return true;
    }
}
