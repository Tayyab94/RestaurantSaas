using RestaurantSaaS.Application.Features.Roles;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalApprovalService(LocalExecutor executor) : IApprovalService
{
    public Task<ApprovalResultDto> RequestAsync(ApprovalRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new RequestApprovalCommand(
                request.ApprovalType, request.BranchId, request.ApproverUserName, request.Pin,
                request.ReferenceType, request.ReferenceId, request.Amount, request.Percent, request.Reason),
            ct);
}
