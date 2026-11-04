using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DayClosing: branch table. Part of the Cash module. See Database Schema.</summary>
public sealed class DayClosing : BranchEntity
{
    public DateOnly BusinessDate { get; set; }
    public string ZReportNumber { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCard { get; set; }
    public decimal TotalOnline { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal TotalExpenses { get; set; }
    public Guid ClosedBy { get; set; }
    public DateTime ClosedAt { get; set; }
    public DayClosingStatus Status { get; set; }
    public string? SnapshotJson { get; set; }
}
