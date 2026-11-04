using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

public interface IOnboardingService
{
    Task<OnboardingChecklistDto> GetChecklistAsync(CancellationToken ct = default);

    Task SetStepAsync(string key, bool isDone, CancellationToken ct = default);
}
