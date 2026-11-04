using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.Application.Common.Paging;

public static class PagingExtensions
{
    /// <summary>
    /// Offset paging: one COUNT query and one page query. The query must already be sorted with a unique tie-breaker (Id).
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest request,
        CancellationToken ct = default)
    {
        if (request.Skip > PageRequest.MaxOffset)
        {
            throw new RequestRejectedException(
                "offset_too_deep",
                $"Pages beyond {PageRequest.MaxOffset} rows are not available. Narrow the filter instead.");
        }

        var total = await query.CountAsync(ct);
        var items = await query.Skip(request.Skip).Take(request.NormalizedPageSize).ToListAsync(ct);

        return new PagedResult<T>(items, request.NormalizedPage, request.NormalizedPageSize, total);
    }
}
