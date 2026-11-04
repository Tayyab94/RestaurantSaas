using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Account;

/// <summary>The signed-in user's own theme and language. "Auto" means no choice was saved yet.</summary>
public sealed record GetMyPreferencesQuery : IRequest<UserPreferencesDto>;

internal sealed class GetMyPreferencesHandler(IApplicationDbContext db, ICurrentUser user)
    : IRequestHandler<GetMyPreferencesQuery, UserPreferencesDto>
{
    public async Task<UserPreferencesDto> HandleAsync(GetMyPreferencesQuery request, CancellationToken ct)
    {
        var userId = user.UserId ?? throw new UnauthorizedException("unauthenticated", "Please sign in to continue.");
        var row = await db.Set<UserPreference>().AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, ct);

        return row is null ? new UserPreferencesDto("Auto", "en") : new UserPreferencesDto(row.Theme.ToString(), row.Language);
    }
}

public sealed record SaveMyPreferencesCommand(string Theme, string Language) : IRequest<bool>;

internal sealed class SaveMyPreferencesValidator : AbstractValidator<SaveMyPreferencesCommand>
{
    public SaveMyPreferencesValidator()
    {
        RuleFor(x => x.Theme).Must(t => Enum.TryParse<UserPreferenceTheme>(t, true, out _)).WithMessage("Choose Light, Dark or Auto.");
        RuleFor(x => x.Language).Must(l => l is "en" or "ur").WithMessage("Choose English or Urdu.");
    }
}

internal sealed class SaveMyPreferencesHandler(IApplicationDbContext db, ICurrentUser user)
    : IRequestHandler<SaveMyPreferencesCommand, bool>
{
    public async Task<bool> HandleAsync(SaveMyPreferencesCommand request, CancellationToken ct)
    {
        var userId = user.UserId ?? throw new UnauthorizedException("unauthenticated", "Please sign in to continue.");
        var row = await db.Set<UserPreference>().FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (row is null)
        {
            row = new UserPreference { TenantId = user.TenantId, UserId = userId, SoundEnabled = true, SoundVolume = 80 };
            db.Set<UserPreference>().Add(row);
        }

        row.Theme = Enum.Parse<UserPreferenceTheme>(request.Theme, true);
        row.Language = request.Language;

        await db.SaveChangesAsync(ct);
        return true;
    }
}
