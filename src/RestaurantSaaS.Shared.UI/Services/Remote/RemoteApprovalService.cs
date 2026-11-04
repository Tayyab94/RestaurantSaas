using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteApprovalService(RemoteApiClient api) : IApprovalService
{
    public Task<ApprovalResultDto> RequestAsync(ApprovalRequest request, CancellationToken ct = default) =>
        api.PostAsync<ApprovalResultDto>("api/v1/approvals", request, authenticated: true, ct);
}
