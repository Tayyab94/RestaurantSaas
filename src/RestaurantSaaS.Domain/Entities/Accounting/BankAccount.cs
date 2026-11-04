using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>BankAccount: tenant table. Part of the Accounting module. See Database Schema.</summary>
public sealed class BankAccount : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public Guid AccountId { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool IsActive { get; set; }

    public ChartOfAccount Account { get; set; } = null!;
}
