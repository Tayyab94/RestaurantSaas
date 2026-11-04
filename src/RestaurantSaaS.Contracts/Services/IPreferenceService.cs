using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

public interface IPreferenceService
{
    Task<UserPreferencesDto> GetAsync(CancellationToken ct = default);

    Task SaveAsync(UserPreferencesDto preferences, CancellationToken ct = default);
}
