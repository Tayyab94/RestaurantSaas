using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>QrOrderSession: branch table. Part of the Floors module. See Database Schema.</summary>
public sealed class QrOrderSession : BranchEntity
{
    public Guid TableId { get; set; }
    public string SessionTokenHash { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerName { get; set; }
    public QrSessionStatus Status { get; set; }
    public Guid? OrderId { get; set; }

    public RestaurantTable Table { get; set; } = null!;
    public Order? Order { get; set; }
}
