using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LoginHistory: table owned by a tenant or by the platform. Part of the Identity module. See Database Schema.</summary>
public sealed class LoginHistory : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public Guid? UserId { get; set; }
    public string UserNameTried { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public Guid? DeviceId { get; set; }
    public DateTime AttemptedAt { get; set; }

    public User? User { get; set; }
    public Device? Device { get; set; }
}
