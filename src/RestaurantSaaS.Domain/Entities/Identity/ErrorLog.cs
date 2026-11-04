using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ErrorLog: table owned by a tenant or by the platform. Part of the Identity module. See Database Schema.</summary>
public sealed class ErrorLog : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public Guid? UserId { get; set; }
    public Guid? DeviceId { get; set; }
    public DateTime OccurredAt { get; set; }

    public User? User { get; set; }
    public Device? Device { get; set; }
}
