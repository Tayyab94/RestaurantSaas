using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Contracts.Paging;
using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Application.Common.Paging;

public static class CursorPagingExtensions
{
    /// <summary>
    /// Cursor (keyset) paging, newest first, sorted by CreatedAt then Id.
    /// Never sorts on Id alone (Guid order differs between PostgreSQL and SQL Server).
    /// Map the entities to DTOs after the call. Verify the Id comparison per provider with integration test PG-06.
    /// </summary>
    public static async Task<CursorResult<TEntity>> ToCursorResultAsync<TEntity>(
        this IQueryable<TEntity> query,
        CursorRequest request,
        CancellationToken ct = default)
        where TEntity : BaseEntity
    {
        var limit = request.NormalizedLimit;

        if (CursorToken.TryDecode(request.Cursor, out var createdAt, out var id))
        {
            query = query.Where(e => e.CreatedAt < createdAt
                                     || (e.CreatedAt == createdAt && e.Id.CompareTo(id) < 0));
        }

        var rows = await query
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.Id)
            .Take(limit + 1)
            .ToListAsync(ct);

        var hasMore = rows.Count > limit;

        if (hasMore)
        {
            rows.RemoveAt(rows.Count - 1);
        }

        var next = hasMore && rows.Count > 0 ? CursorToken.Encode(rows[^1].CreatedAt, rows[^1].Id) : null;

        return new CursorResult<TEntity>(rows, next, hasMore);
    }
}

/// <summary>The cursor is an opaque base64 text holding the last row's CreatedAt and Id.</summary>
internal static class CursorToken
{
    public static string Encode(DateTime createdAt, Guid id)
    {
        var text = $"{createdAt.Ticks.ToString(CultureInfo.InvariantCulture)}|{id:D}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }

    public static bool TryDecode(string? cursor, out DateTime createdAt, out Guid id)
    {
        createdAt = default;
        id = default;

        if (string.IsNullOrWhiteSpace(cursor))
        {
            return false;
        }

        try
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(cursor)).Split('|');

            if (parts.Length != 2
                || !long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
                || !Guid.TryParse(parts[1], out id))
            {
                return false;
            }

            createdAt = new DateTime(ticks, DateTimeKind.Utc);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
