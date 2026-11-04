using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>UserPreference: table owned by a tenant or by the platform. Part of the Identity module. See Database Schema.</summary>
public sealed class UserPreference : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }
    public UserPreferenceTheme Theme { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool SoundEnabled { get; set; }
    public int SoundVolume { get; set; }
    public Guid? DefaultBranchId { get; set; }
    public string? DefaultScreen { get; set; }
    public string? NumberFormat { get; set; }
    public string? ShortcutsJson { get; set; }

    public User User { get; set; } = null!;
    public Branch? DefaultBranch { get; set; }
}
