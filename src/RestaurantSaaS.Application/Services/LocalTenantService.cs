using RestaurantSaaS.Application.Features.Tenants;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalTenantService(LocalExecutor executor) : ITenantService
{
    public Task<PagedResult<TenantDto>> GetPageAsync(PageRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new GetTenantsPageQuery(request), ct);
}
