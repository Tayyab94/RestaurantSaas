using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteLicenseService(RemoteApiClient api) : ILicenseService
{
    public Task<LicenseStatusDto> GetStatusAsync(CancellationToken ct = default) =>
        api.GetAsync<LicenseStatusDto>("api/v1/license/status", ct);

    public Task<LicenseStatusDto> ActivateAsync(ActivateLicenseRequest request, CancellationToken ct = default) =>
        api.PostAsync<LicenseStatusDto>("api/v1/license/activate", request, true, ct);

    public Task<LicenseStatusDto> StartTrialAsync(StartTrialRequest request, CancellationToken ct = default) =>
        api.PostAsync<LicenseStatusDto>("api/v1/license/trial", request, true, ct);

    public Task<GeneratedLicenseKeysDto> GenerateKeysAsync(GenerateLicenseKeysRequest request, CancellationToken ct = default) =>
        api.PostAsync<GeneratedLicenseKeysDto>("api/v1/license/keys", request, true, ct);

    public Task<PagedResult<LicenseKeyListItemDto>> GetKeysPageAsync(PageRequest request, CancellationToken ct = default) =>
        api.GetAsync<PagedResult<LicenseKeyListItemDto>>($"api/v1/license/keys?{RemoteQuery.For(request)}", ct);

    public async Task<IReadOnlyList<PlanOptionDto>> GetPlansAsync(CancellationToken ct = default) =>
        await api.GetAsync<List<PlanOptionDto>>("api/v1/license/plans", ct);

    // ADD inside the class:
    public Task<LicenseStatusDto> DeactivateAsync(CancellationToken ct = default) =>
        api.PostAsync<LicenseStatusDto>("api/v1/license/deactivate", new { }, true, ct);

    public async Task ResetKeyAsync(Guid licenseId, CancellationToken ct = default) =>
        await api.PostAsync<bool>($"api/v1/license/keys/{licenseId}/reset", new { }, true, ct);
}
