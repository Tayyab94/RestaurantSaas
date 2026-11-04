using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PlatformSetting: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class PlatformSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public string? Description { get; set; }
}
