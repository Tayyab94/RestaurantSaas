using RestaurantSaaS.Application.Features.Account;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalPreferenceService(LocalExecutor executor) : IPreferenceService
{
    public Task<UserPreferencesDto> GetAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetMyPreferencesQuery(), ct);

    public async Task SaveAsync(UserPreferencesDto preferences, CancellationToken ct = default) =>
        await executor.SendAsync(new SaveMyPreferencesCommand(preferences.Theme, preferences.Language), ct);
}
