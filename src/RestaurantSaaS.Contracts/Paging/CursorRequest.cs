namespace RestaurantSaaS.Contracts.Paging;

/// <summary>
/// Cursor (keyset) paging request. Used by large, growing, time-ordered lists
/// (orders, invoices, ledgers, audit logs) and by sync downloads.
/// </summary>
public sealed record CursorRequest(string? Cursor = null, int Limit = CursorRequest.DefaultLimit)
{
    public const int DefaultLimit = 50;
    public const int MaxLimit = 100;

    public int NormalizedLimit => Limit < 1 ? DefaultLimit : Math.Min(Limit, MaxLimit);
}
