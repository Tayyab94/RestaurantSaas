using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Paging;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Audit;

/// <summary>
/// Writes a line of the audit log for something that matters for security: users, roles, PINs, approvals.
/// It only adds the row; the calling handler saves it together with its own change, so both succeed or both fail.
/// Never put a password, a PIN or a token into the values.
/// </summary>
internal sealed class AuditTrail(IApplicationDbContext db, ICurrentUser user, IRequestInfo requestInfo, IClock clock)
{
    private static readonly JsonSerializerOptions Json = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public void Add(string action, string entityName, Guid entityId, object? oldValues = null, object? newValues = null, Guid? branchId = null)
    {
        // Platform actions (no business) are not part of a business's audit log.
        if (user.TenantId is null)
        {
            return;
        }

        db.Set<AuditLog>().Add(new AuditLog
        {
            UserId = user.UserId,
            BranchId = branchId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValuesJson = oldValues is null ? null : JsonSerializer.Serialize(oldValues, Json),
            NewValuesJson = newValues is null ? null : JsonSerializer.Serialize(newValues, Json),
            IpAddress = requestInfo.IpAddress,
            OccurredAt = clock.UtcNow,
        });
    }
}

/// <summary>Who did what, newest first.</summary>
[RequirePermission(Permissions.AuditLogView)]
public sealed record GetAuditLogPageQuery(PageRequest Page) : IRequest<PagedResult<AuditLogItemDto>>;

internal sealed class GetAuditLogPageHandler(IApplicationDbContext db)
    : IRequestHandler<GetAuditLogPageQuery, PagedResult<AuditLogItemDto>>
{
    public async Task<PagedResult<AuditLogItemDto>> HandleAsync(GetAuditLogPageQuery request, CancellationToken ct)
    {
        IQueryable<AuditLog> query = db.Set<AuditLog>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Page.Search))
        {
            var term = request.Page.Search.Trim();
            query = query.Where(a => a.Action.Contains(term) || a.EntityName.Contains(term) || (a.User != null && a.User.UserName.Contains(term)));
        }

        return await query
            .OrderByDescending(a => a.OccurredAt).ThenByDescending(a => a.Id)
            .Select(a => new AuditLogItemDto(
                a.Id,
                a.OccurredAt,
                a.User != null ? a.User.UserName : null,
                a.Action,
                a.EntityName,
                a.EntityId,
                a.NewValuesJson,
                a.IpAddress))
            .ToPagedResultAsync(request.Page, ct);
    }
}
