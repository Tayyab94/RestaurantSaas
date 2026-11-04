using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>AccountingPeriod: tenant table. Part of the Accounting module. See Database Schema.</summary>
public sealed class AccountingPeriod : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public AccountingPeriodStatus Status { get; set; }
    public Guid? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
}
