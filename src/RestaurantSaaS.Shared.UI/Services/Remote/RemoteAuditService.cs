using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteAuditService(RemoteApiClient api) : IAuditService
{
    public Task<PagedResult<AuditLogItemDto>> GetPageAsync(PageRequest request, CancellationToken ct = default) =>
        api.GetAsync<PagedResult<AuditLogItemDto>>($"api/v1/audit-log?{RemoteQuery.For(request)}", ct);
}
