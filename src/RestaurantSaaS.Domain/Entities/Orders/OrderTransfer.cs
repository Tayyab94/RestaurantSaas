using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OrderTransfer: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class OrderTransfer : BranchEntity
{
    public Guid OrderId { get; set; }
    public OrderTransferTransferType TransferType { get; set; }
    public Guid? FromTableId { get; set; }
    public Guid? ToTableId { get; set; }
    public Guid? FromUserId { get; set; }
    public Guid? ToUserId { get; set; }
    public Guid TransferredBy { get; set; }
    public DateTime TransferredAt { get; set; }
    public string? Reason { get; set; }

    public Order Order { get; set; } = null!;
    public RestaurantTable? FromTable { get; set; }
    public RestaurantTable? ToTable { get; set; }
    public User? FromUser { get; set; }
    public User? ToUser { get; set; }
}
