using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteSetupService(RemoteApiClient api) : ISetupService
{
    public Task<SetupStatusDto> GetStatusAsync(CancellationToken ct = default) =>
        api.GetAsync<SetupStatusDto>("api/v1/setup/status", authenticated: false, ct);
}
