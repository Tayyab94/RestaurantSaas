using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DataExport: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class DataExport : TenantEntity
{
    public string ExportType { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public long SizeBytes { get; set; }
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
