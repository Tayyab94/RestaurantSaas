using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Contracts.Services;

public interface ILicenseService
{
    Task<LicenseStatusDto> GetStatusAsync(CancellationToken ct = default);
    Task<LicenseStatusDto> ActivateAsync(ActivateLicenseRequest request, CancellationToken ct = default);
    Task<LicenseStatusDto> StartTrialAsync(StartTrialRequest request, CancellationToken ct = default);
    Task<GeneratedLicenseKeysDto> GenerateKeysAsync(GenerateLicenseKeysRequest request, CancellationToken ct = default);
    Task<PagedResult<LicenseKeyListItemDto>> GetKeysPageAsync(PageRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<PlanOptionDto>> GetPlansAsync(CancellationToken ct = default);

    /// Frees this PC's activation on the License Server and removes the license file.
    /// The same key can be activated again later.
    Task<LicenseStatusDto> DeactivateAsync(CancellationToken ct = default);

    /// SuperAdmin: frees every PC of a key (for example when the PC was lost).
    Task ResetKeyAsync(Guid licenseId, CancellationToken ct = default);
}
