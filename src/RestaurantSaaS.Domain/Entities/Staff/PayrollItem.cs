using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>PayrollItem: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class PayrollItem : BranchEntity
{
    public Guid PayrollId { get; set; }
    public PayrollItemItemType ItemType { get; set; }
    public Guid? PayComponentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }

    public Payroll Payroll { get; set; } = null!;
    public PayComponent? PayComponent { get; set; }
}
