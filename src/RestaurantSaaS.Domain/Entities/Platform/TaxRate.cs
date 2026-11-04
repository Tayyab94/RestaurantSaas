using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TaxRate: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class TaxRate : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public TaxRateTaxType TaxType { get; set; }
    public TaxRateAppliesTo AppliesTo { get; set; }
    public bool IsInclusive { get; set; }
    public bool IsDefault { get; set; }
    public string? FbrTaxCode { get; set; }
}
