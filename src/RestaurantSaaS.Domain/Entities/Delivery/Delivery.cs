using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Delivery: branch table. Part of the Delivery module. See Database Schema.</summary>
public sealed class Delivery : BranchEntity
{
    public Guid OrderId { get; set; }
    public DeliveryDeliveryType DeliveryType { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid? DeliveryAreaId { get; set; }
    public string? Landmark { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal DeliveryCharge { get; set; }
    public Guid? RiderId { get; set; }
    public decimal RiderCommission { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailReason { get; set; }
    public decimal CashToCollect { get; set; }
    public decimal CashCollected { get; set; }
    public string? ThirdPartyName { get; set; }
    public string? ExternalReference { get; set; }
    public Guid? RiderSettlementId { get; set; }

    public Order Order { get; set; } = null!;
    public Customer? Customer { get; set; }
    public DeliveryArea? DeliveryArea { get; set; }
    public Rider? Rider { get; set; }
    public RiderSettlement? RiderSettlement { get; set; }
}
