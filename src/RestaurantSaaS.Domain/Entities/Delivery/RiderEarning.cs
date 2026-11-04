using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RiderEarning: branch table. Part of the Delivery module. See Database Schema.</summary>
public sealed class RiderEarning : BranchEntity
{
    public Guid RiderId { get; set; }
    public Guid? DeliveryId { get; set; }
    public DateOnly BusinessDate { get; set; }
    public RiderEarningEarningType EarningType { get; set; }
    public decimal Amount { get; set; }
    public Guid? RiderSettlementId { get; set; }
    public Guid? PayrollItemId { get; set; }
    public RiderEarningStatus Status { get; set; }
    public string? Notes { get; set; }

    public Rider Rider { get; set; } = null!;
    public Delivery? Delivery { get; set; }
    public RiderSettlement? RiderSettlement { get; set; }
    public PayrollItem? PayrollItem { get; set; }
}
