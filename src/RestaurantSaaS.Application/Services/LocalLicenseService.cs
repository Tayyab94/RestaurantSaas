using RestaurantSaaS.Application.Features.Licensing;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalLicenseService(LocalExecutor executor) : ILicenseService
{
    public Task<LicenseStatusDto> GetStatusAsync(CancellationToken ct = default) =>
        executor.SendAsync(new GetLicenseStatusQuery(), ct);

    public Task<LicenseStatusDto> ActivateAsync(ActivateLicenseRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new ActivateLicenseCommand(request.Key, request.UserName), ct);

    public Task<LicenseStatusDto> StartTrialAsync(StartTrialRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new StartTrialCommand(request.UserName, request.Phone, request.Email), ct);

    public Task<GeneratedLicenseKeysDto> GenerateKeysAsync(GenerateLicenseKeysRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new GenerateLicenseKeysCommand(request), ct);

    public Task<PagedResult<LicenseKeyListItemDto>> GetKeysPageAsync(PageRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new GetLicenseKeysPageQuery(request), ct);

    public async Task<IReadOnlyList<PlanOptionDto>> GetPlansAsync(CancellationToken ct = default) =>
        await executor.SendAsync(new GetPlanOptionsQuery(), ct);

    // ADD inside the class:
    public Task<LicenseStatusDto> DeactivateAsync(CancellationToken ct = default) =>
        executor.SendAsync(new DeactivateLicenseCommand(), ct);

    public async Task ResetKeyAsync(Guid licenseId, CancellationToken ct = default) =>
        await executor.SendAsync(new ResetLicenseKeyCommand(licenseId), ct);
}
