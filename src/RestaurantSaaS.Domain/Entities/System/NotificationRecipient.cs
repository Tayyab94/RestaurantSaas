using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>NotificationRecipient: branch table. Part of the System module. See Database Schema.</summary>
public sealed class NotificationRecipient : BranchEntity
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    public Notification Notification { get; set; } = null!;
    public User User { get; set; } = null!;
}
