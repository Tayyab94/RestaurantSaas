using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Notification: branch table. Part of the System module. See Database Schema.</summary>
public sealed class Notification : BranchEntity
{
    public NotificationNotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? PayloadJson { get; set; }
    public NotificationPriority Priority { get; set; }
    public NotificationTargetType TargetType { get; set; }
    public Guid? TargetUserId { get; set; }
    public string? TargetRole { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public User? TargetUser { get; set; }
}
