using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RestoreLog: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class RestoreLog : TenantEntity
{
    public Guid? BackupLogId { get; set; }
    public string SourceFile { get; set; } = string.Empty;
    public Guid RestoredBy { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string Result { get; set; } = string.Empty;
    public RestoreLogLicenseOutcome LicenseOutcome { get; set; }
    public string? Notes { get; set; }

    public BackupLog? BackupLog { get; set; }
}
