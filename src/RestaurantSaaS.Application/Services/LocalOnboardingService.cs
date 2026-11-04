using RestaurantSaaS.Application.Features.Onboarding;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalOnboardingService(LocalExecutor executor) : IOnboardingService
{
    public Task<OnboardingChecklistDto> GetChecklistAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetOnboardingChecklistQuery(), ct);

    public async Task SetStepAsync(string key, bool isDone, CancellationToken ct = default) =>
        await executor.SendAsync(new SetOnboardingStepCommand(key, isDone), ct);
}
