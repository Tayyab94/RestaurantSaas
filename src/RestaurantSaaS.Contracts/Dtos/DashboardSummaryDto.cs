namespace RestaurantSaaS.Contracts.Dtos;

/// <summary>Everything the dashboard needs in one call.</summary>
public sealed record DashboardSummaryDto(
    bool IsDemo,
    string Currency,
    DateOnly BusinessDate,
    decimal TodaySales,
    decimal YesterdaySales,
    int OpenOrders,
    int TablesOccupied,
    int TablesTotal,
    int PendingDeliveries,
    decimal CashInDrawer,
    int LowStockItems,
    IReadOnlyList<SalesPointDto> SalesLast7Days,
    IReadOnlyList<OrderTypeShareDto> OrderTypeShare,
    IReadOnlyList<TopItemDto> TopItems,
    IReadOnlyList<RecentOrderDto> RecentOrders);

public sealed record SalesPointDto(DateOnly Date, decimal Amount);

public sealed record OrderTypeShareDto(string Name, int Count);

public sealed record TopItemDto(string Name, int Quantity, decimal Amount);

public sealed record RecentOrderDto(string Number, string Type, string Reference, decimal Total, string Status, DateTime At);
