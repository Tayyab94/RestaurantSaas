namespace RestaurantSaaS.Contracts.Paging;

public sealed record CursorResult<T>(IReadOnlyList<T> Items, string? NextCursor, bool HasMore)
{
    public static CursorResult<T> Empty() => new([], null, false);
}
