using RestaurantSaaS.Application.Features.Dashboard;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalDashboardService(LocalExecutor executor) : IDashboardService
{
    public Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetDashboardSummaryQuery(), ct);
}
