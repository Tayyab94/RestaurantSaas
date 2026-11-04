using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>UserTwoFactor: table owned by a tenant or by the platform. Part of the Identity module. See Database Schema.</summary>
public sealed class UserTwoFactor : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }
    public TwoFactorMethod Method { get; set; }
    public string SecretEncrypted { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string RecoveryCodesHash { get; set; } = string.Empty;
    public DateTime? EnabledAt { get; set; }

    public User User { get; set; } = null!;
}
