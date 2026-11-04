using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemotePreferenceService(RemoteApiClient api) : IPreferenceService
{
    public Task<UserPreferencesDto> GetAsync(CancellationToken ct = default) =>
        api.GetAsync<UserPreferencesDto>("api/v1/account/preferences", ct);

    public Task SaveAsync(UserPreferencesDto preferences, CancellationToken ct = default) =>
        api.PutAsync("api/v1/account/preferences", preferences, ct);
}
