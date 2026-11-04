using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Shift: branch table. Part of the Cash module. See Database Schema.</summary>
public sealed class Shift : BranchEntity
{
    public string ShiftNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public Guid CashierId { get; set; }
    public Guid? DeviceId { get; set; }
    public DateOnly BusinessDate { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal ExpectedCash { get; set; }
    public decimal? ClosingCash { get; set; }
    public decimal? CashDifference { get; set; }
    public decimal TotalSales { get; set; }
    public ShiftStatus Status { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? Notes { get; set; }

    public User Cashier { get; set; } = null!;
    public Device? Device { get; set; }
}
