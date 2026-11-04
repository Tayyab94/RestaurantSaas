using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ImpersonationSession: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class ImpersonationSession : BaseEntity
{
    public Guid SuperAdminUserId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public int ActionsCount { get; set; }

    public User SuperAdminUser { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
