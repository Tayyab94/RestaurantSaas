using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Application.Features.Dashboard;

[RequirePermission(Permissions.DashboardView)]
public sealed record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

/// <summary>
/// Returns DEMO numbers so the dashboard UI can be built and shown now.
/// It is replaced by real queries (orders, tables, payments, stock) as those modules are built.
/// </summary>
internal sealed class GetDashboardSummaryHandler(IClock clock)
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public Task<DashboardSummaryDto> HandleAsync(GetDashboardSummaryQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);

        decimal[] amounts = [182_400, 205_900, 167_300, 231_800, 248_600, 212_450, 276_100];
        var sales = amounts.Select((amount, i) => new SalesPointDto(today.AddDays(i - 6), amount)).ToList();

        var summary = new DashboardSummaryDto(
            IsDemo: true,
            Currency: "PKR",
            BusinessDate: today,
            TodaySales: 276_100,
            YesterdaySales: 212_450,
            OpenOrders: 14,
            TablesOccupied: 11,
            TablesTotal: 22,
            PendingDeliveries: 5,
            CashInDrawer: 84_300,
            LowStockItems: 6,
            SalesLast7Days: sales,
            OrderTypeShare:
            [
                new("Dine-in", 96),
                new("Takeaway", 54),
                new("Delivery", 38),
            ],
            TopItems:
            [
                new("Zinger Burger", 64, 57_600),
                new("Chicken Karahi (Full)", 22, 52_800),
                new("Beef Pizza (Large)", 31, 46_500),
                new("Club Sandwich", 38, 28_500),
                new("Mint Margarita", 71, 21_300),
            ],
            RecentOrders:
            [
                new("D2-0148", "Dine-in", "Table 7", 3_450, "Preparing", clock.UtcNow.AddMinutes(-4)),
                new("D2-0147", "Delivery", "0300-1234567", 2_150, "Out for delivery", clock.UtcNow.AddMinutes(-11)),
                new("D1-0146", "Takeaway", "Walk-in #42", 1_280, "Ready", clock.UtcNow.AddMinutes(-15)),
                new("D1-0145", "Dine-in", "Table 3", 5_920, "Served", clock.UtcNow.AddMinutes(-26)),
                new("D2-0144", "Dine-in", "Table 12", 4_100, "Completed", clock.UtcNow.AddMinutes(-41)),
                new("D1-0143", "Takeaway", "Walk-in #41", 960, "Cancelled", clock.UtcNow.AddMinutes(-52)),
            ]);

        return Task.FromResult(summary);
    }
}
