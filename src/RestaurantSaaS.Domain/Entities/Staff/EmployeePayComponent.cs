using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>EmployeePayComponent: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class EmployeePayComponent : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public Guid PayComponentId { get; set; }
    public decimal Value { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }

    public Employee Employee { get; set; } = null!;
    public PayComponent PayComponent { get; set; } = null!;
}
