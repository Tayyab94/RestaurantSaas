using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>VerificationCode: platform table (no tenant). Part of the Identity module. See Database Schema.</summary>
public sealed class VerificationCode : BaseEntity
{
    public VerificationCodePurpose Purpose { get; set; }
    public string TargetHash { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? IpAddress { get; set; }
}
