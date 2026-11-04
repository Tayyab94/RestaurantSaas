using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PushToken: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class PushToken : TenantEntity
{
    public Guid UserId { get; set; }
    public Guid? DeviceId { get; set; }
    public PushTokenPlatform Platform { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }

    public User User { get; set; } = null!;
    public Device? Device { get; set; }
}
