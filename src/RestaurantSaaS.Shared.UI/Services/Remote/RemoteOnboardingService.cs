using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteOnboardingService(RemoteApiClient api) : IOnboardingService
{
    public Task<OnboardingChecklistDto> GetChecklistAsync(CancellationToken ct = default) =>
        api.GetAsync<OnboardingChecklistDto>("api/v1/onboarding", ct);

    public Task SetStepAsync(string key, bool isDone, CancellationToken ct = default) =>
        api.PutAsync($"api/v1/onboarding/{Uri.EscapeDataString(key)}", new SetOnboardingStepRequest(isDone), ct);
}
