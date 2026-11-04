using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Contracts.Services;

public interface IAuditService
{
    Task<PagedResult<AuditLogItemDto>> GetPageAsync(PageRequest request, CancellationToken ct = default);
}
