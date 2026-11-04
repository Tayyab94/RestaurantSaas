namespace RestaurantSaaS.Contracts.Paging;

/// <summary>
/// Offset paging request (page numbers, total count). Used by back-office lists.
/// Large time-ordered lists use <see cref="CursorRequest"/> instead.
/// </summary>
public sealed record PageRequest(
    int Page = 1,
    int PageSize = PageRequest.DefaultPageSize,
    string? Sort = null,
    bool Desc = false,
    string? Search = null)
{
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 100;
    public const int MaxOffset = 10_000;

    public int NormalizedPage => Page < 1 ? 1 : Page;

    public int NormalizedPageSize => PageSize < 1 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize);

    public int Skip => (NormalizedPage - 1) * NormalizedPageSize;
}
