using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>BackupLog: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class BackupLog : TenantEntity
{
    public BackupLogBackupType BackupType { get; set; }
    public BackupLogLocation Location { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
