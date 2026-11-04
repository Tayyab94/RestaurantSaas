using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DailySalesSummary: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class DailySalesSummary : BranchEntity
{
    public DateOnly BusinessDate { get; set; }
    public int OrderCount { get; set; }
    public decimal GrossSales { get; set; }
    public decimal Discounts { get; set; }
    public decimal Tax { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal NetSales { get; set; }
    public decimal Returns { get; set; }
    public string? ByOrderTypeJson { get; set; }
    public string? ByPaymentJson { get; set; }
    public string? HourlyJson { get; set; }
    public string? TopItemsJson { get; set; }
    public DateTime GeneratedAt { get; set; }
}
