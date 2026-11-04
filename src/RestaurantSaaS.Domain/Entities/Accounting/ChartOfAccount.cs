using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ChartOfAccount: tenant table. Part of the Accounting module. See Database Schema.</summary>
public sealed class ChartOfAccount : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public Guid? ParentId { get; set; }
    public ChartOfAccountAccountType AccountType { get; set; }
    public bool IsGroup { get; set; }
    public bool IsSystem { get; set; }
    public bool IsCash { get; set; }
    public bool IsBank { get; set; }
    public bool IsActive { get; set; }

    public ChartOfAccount? Parent { get; set; }
}
