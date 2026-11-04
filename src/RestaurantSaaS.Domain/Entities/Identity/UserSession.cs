using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>UserSession: table owned by a tenant or by the platform. Part of the Identity module. See Database Schema.</summary>
public sealed class UserSession : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }
    public Guid? DeviceId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsActive { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? EndReason { get; set; }

    public User User { get; set; } = null!;
    public Device? Device { get; set; }
}
