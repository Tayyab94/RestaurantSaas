using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>SupplierEvaluation: tenant table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class SupplierEvaluation : TenantEntity
{
    public Guid SupplierId { get; set; }
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public int OrdersCount { get; set; }
    public decimal OnTimePercent { get; set; }
    public decimal RejectionPercent { get; set; }
    public decimal PriceScore { get; set; }
    public int Rating { get; set; }
    public string? Notes { get; set; }
    public DateTime GeneratedAt { get; set; }

    public Supplier Supplier { get; set; } = null!;
}
