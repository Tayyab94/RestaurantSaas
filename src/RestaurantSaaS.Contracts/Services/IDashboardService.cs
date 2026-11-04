using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

/// <summary>
/// What the UI calls. Two implementations exist: Local (in-process, used by the Web server UI)
/// and Remote (HTTP to the API, used by the desktop app).
/// </summary>
public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
