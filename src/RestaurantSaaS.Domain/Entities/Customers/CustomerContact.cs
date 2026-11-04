using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CustomerContact: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class CustomerContact : TenantEntity
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }

    public Customer Customer { get; set; } = null!;
}
