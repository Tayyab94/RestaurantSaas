using RestaurantSaaS.Application.Features.Setup;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalSetupService(LocalExecutor executor) : ISetupService
{
    public Task<SetupStatusDto> GetStatusAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetSetupStatusQuery(), ct);
}
