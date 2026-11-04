using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Common.Paging;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Tenants;

/// <summary>Offset-paged tenant list (platform administration, SuperAdmin only). The pattern for every back-office list.</summary>
[RequirePermission(Permissions.TenantView)]
public sealed record GetTenantsPageQuery(PageRequest Page) : IRequest<PagedResult<TenantDto>>;

internal sealed class GetTenantsPageHandler(IApplicationDbContext db)
    : IRequestHandler<GetTenantsPageQuery, PagedResult<TenantDto>>
{
    public async Task<PagedResult<TenantDto>> HandleAsync(GetTenantsPageQuery request, CancellationToken ct)
    {
        var page = request.Page;
        IQueryable<Tenant> query = db.Tenants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var term = page.Search.Trim();
            query = query.Where(t => t.Name.Contains(term) || t.Code.Contains(term));
        }

        var rows = await ApplySort(query, page)
            .Select(t => new Row(t.Id, t.Code, t.Name, t.BusinessType, t.Status, t.DeploymentMode, t.CreatedAt))
            .ToPagedResultAsync(page, ct);

        var items = rows.Items
            .Select(r => new TenantDto(r.Id, r.Code, r.Name, r.BusinessType.ToString(), r.Status.ToString(), r.DeploymentMode.ToString(), r.CreatedAt))
            .ToList();

        return new PagedResult<TenantDto>(items, rows.Page, rows.PageSize, rows.TotalCount);
    }

    // Only whitelisted columns can be sorted, and Id is always the last sort key (stable order).
    private static IQueryable<Tenant> ApplySort(IQueryable<Tenant> query, PageRequest page) =>
        (page.Sort?.ToLowerInvariant(), page.Desc) switch
        {
            (null, _) => query.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id),
            ("name", false) => query.OrderBy(t => t.Name).ThenBy(t => t.Id),
            ("name", true) => query.OrderByDescending(t => t.Name).ThenByDescending(t => t.Id),
            ("code", false) => query.OrderBy(t => t.Code).ThenBy(t => t.Id),
            ("code", true) => query.OrderByDescending(t => t.Code).ThenByDescending(t => t.Id),
            ("createdat", false) => query.OrderBy(t => t.CreatedAt).ThenBy(t => t.Id),
            ("createdat", true) => query.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id),
            _ => throw new RequestRejectedException("invalid_sort", $"Sorting by '{page.Sort}' is not allowed."),
        };

    private sealed record Row(
        Guid Id,
        string Code,
        string Name,
        BusinessType BusinessType,
        TenantStatus Status,
        DeploymentMode DeploymentMode,
        DateTime CreatedAt);
}
