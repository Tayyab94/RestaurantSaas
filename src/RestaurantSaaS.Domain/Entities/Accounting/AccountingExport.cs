using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>AccountingExport: tenant table. Part of the Accounting module. See Database Schema.</summary>
public sealed class AccountingExport : TenantEntity
{
    public string ExportType { get; set; } = string.Empty;
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public string Format { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public Guid ExportedBy { get; set; }
    public DateTime ExportedAt { get; set; }
}
