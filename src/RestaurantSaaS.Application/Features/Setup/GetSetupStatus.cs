using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Setup;

/// <summary>
/// Open to everyone. Tells the sign-in screen where this installation stands:
/// the first run of a desktop or on-premise installation goes to the setup screen, and the single business code is filled in.
/// </summary>
[AllowAnonymousRequest]
public sealed record GetSetupStatusQuery : IRequest<SetupStatusDto>;

internal sealed class GetSetupStatusHandler(IApplicationDbContext db, IDeploymentInfo deployment)
    : IRequestHandler<GetSetupStatusQuery, SetupStatusDto>
{
    public async Task<SetupStatusDto> HandleAsync(GetSetupStatusQuery request, CancellationToken ct)
    {
        var codes = await db.Tenants.IgnoreQueryFilters()
            .Where(t => !t.IsDeleted)
            .Select(t => t.Code)
            .Take(2)
            .ToListAsync(ct);

        var isCloud = deployment.Mode == DeploymentMode.Cloud;
        var hasBusiness = codes.Count > 0;
        var single = !isCloud && codes.Count == 1 ? codes[0] : null;

        return new SetupStatusDto(deployment.Mode.ToString(), hasBusiness, isCloud || !hasBusiness, single);
    }
}
