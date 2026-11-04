using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RiderSettlement: branch table. Part of the Delivery module. See Database Schema.</summary>
public sealed class RiderSettlement : BranchEntity
{
    public Guid RiderId { get; set; }
    public DateOnly BusinessDate { get; set; }
    public int TotalDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public decimal CashCollected { get; set; }
    public decimal CashHandedOver { get; set; }
    public decimal Commission { get; set; }
    public decimal Difference { get; set; }
    public decimal ShortageAmount { get; set; }
    public RiderSettlementShortageHandling ShortageHandling { get; set; }
    public Guid? ShortageApprovedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid SettledBy { get; set; }
    public DateTime SettledAt { get; set; }
    public string? Notes { get; set; }

    public Rider Rider { get; set; } = null!;
}
