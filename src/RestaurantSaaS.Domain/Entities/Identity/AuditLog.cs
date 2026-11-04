using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>AuditLog: tenant table. Part of the Identity module. See Database Schema.</summary>
public sealed class AuditLog : TenantEntity
{
    public Guid? BranchId { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? IpAddress { get; set; }
    public Guid? DeviceId { get; set; }
    public DateTime OccurredAt { get; set; }

    public Branch? Branch { get; set; }
    public User? User { get; set; }
    public Device? Device { get; set; }
}
