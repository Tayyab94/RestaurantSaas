using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Device: branch table. Part of the Platform module. See Database Schema.</summary>
public sealed class Device : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public DeviceDeviceType DeviceType { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string? AppVersion { get; set; }
    public DeviceMode Mode { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public string? IpAddress { get; set; }
    public bool IsActive { get; set; }
    public bool IsLicensedSeat { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? BlockedReason { get; set; }
}
