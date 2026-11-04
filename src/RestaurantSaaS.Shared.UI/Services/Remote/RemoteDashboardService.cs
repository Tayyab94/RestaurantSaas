using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteDashboardService(RemoteApiClient api) : IDashboardService
{
    public Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default) =>
        api.GetAsync<DashboardSummaryDto>("api/v1/dashboard/summary", ct);
}
