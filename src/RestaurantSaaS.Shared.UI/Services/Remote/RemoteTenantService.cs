using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteTenantService(RemoteApiClient api) : ITenantService
{
    public Task<PagedResult<TenantDto>> GetPageAsync(PageRequest request, CancellationToken ct = default) =>
        api.GetAsync<PagedResult<TenantDto>>($"api/v1/tenants?{RemoteQuery.For(request)}", ct);
}
