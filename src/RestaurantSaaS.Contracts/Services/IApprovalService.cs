using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

public interface IApprovalService
{
    /// <summary>Checks the manager's PIN and limits and writes the approval to the log. Throws ServiceException when refused.</summary>
    Task<ApprovalResultDto> RequestAsync(ApprovalRequest request, CancellationToken ct = default);
}
