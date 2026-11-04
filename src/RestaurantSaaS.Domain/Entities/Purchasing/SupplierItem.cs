using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierItem: tenant table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierItem : TenantEntity
{
    public Guid SupplierId { get; set; }
    public Guid RawMaterialId { get; set; }
    public string? SupplierSku { get; set; }
    public Guid PurchaseUnitId { get; set; }
    public decimal Price { get; set; }
    public decimal? MinOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public bool IsPreferred { get; set; }
    public bool IsActive { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
    public Unit PurchaseUnit { get; set; } = null!;
}
