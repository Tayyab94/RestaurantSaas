using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DeliveryStatusHistory: branch table. Part of the Delivery module. See Database Schema.</summary>
public sealed class DeliveryStatusHistory : BranchEntity
{
    public Guid DeliveryId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Note { get; set; }

    public Delivery Delivery { get; set; } = null!;
}
