using RestaurantSaaS.Application.Features.Audit;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalAuditService(LocalExecutor executor) : IAuditService
{
    public Task<PagedResult<AuditLogItemDto>> GetPageAsync(PageRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new GetAuditLogPageQuery(request), ct);
}
