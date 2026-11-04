using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>AppUpdateLog: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class AppUpdateLog : TenantEntity
{
    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;
    public string? BackupPath { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public AppUpdateLogResult Result { get; set; }
    public string? ErrorMessage { get; set; }
}
