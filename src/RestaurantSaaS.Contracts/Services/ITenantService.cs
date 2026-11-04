using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Contracts.Services;

public interface ITenantService
{
    Task<PagedResult<TenantDto>> GetPageAsync(PageRequest request, CancellationToken ct = default);
}
