using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>InstallationInfo: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class InstallationInfo : TenantEntity
{
    public Guid InstallationId { get; set; }
    public DeploymentMode DeploymentMode { get; set; }
    public InstallationRole Role { get; set; }
    public DateTime InstalledAt { get; set; }
    public string InstalledVersion { get; set; } = string.Empty;
    public string? ServerAddress { get; set; }
    public string? MachineName { get; set; }
    public string? ServiceName { get; set; }
}
