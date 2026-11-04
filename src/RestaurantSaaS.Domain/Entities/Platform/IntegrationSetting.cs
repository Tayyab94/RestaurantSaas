using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>IntegrationSetting: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class IntegrationSetting : TenantEntity
{
    public Guid? BranchId { get; set; }
    public IntegrationProvider Provider { get; set; }
    public string ConfigEncrypted { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastSyncAt { get; set; }

    public Branch? Branch { get; set; }
}
