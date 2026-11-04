using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierContact: tenant table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierContact : TenantEntity
{
    public Guid SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }

    public Supplier Supplier { get; set; } = null!;
}
