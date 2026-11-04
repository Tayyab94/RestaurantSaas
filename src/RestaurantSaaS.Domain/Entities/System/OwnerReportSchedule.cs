using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>OwnerReportSchedule: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class OwnerReportSchedule : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public OwnerReportScheduleReportType ReportType { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public TimeOnly SendTime { get; set; }
    public Guid? BranchId { get; set; }
    public bool IsActive { get; set; }

    public Branch? Branch { get; set; }
}
