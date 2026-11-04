using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ImportJob: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class ImportJob : TenantEntity
{
    public ImportJobImportType ImportType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public ImportJobStatus Status { get; set; }
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int ErrorRows { get; set; }
    public string? ErrorFileUrl { get; set; }
    public Guid ImportedBy { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
