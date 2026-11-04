using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ExpenseCategory: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class ExpenseCategory : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }

    public ExpenseCategory? Parent { get; set; }
}
