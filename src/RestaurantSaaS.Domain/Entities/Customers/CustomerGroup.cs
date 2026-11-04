using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CustomerGroup: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class CustomerGroup : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal? DiscountPercent { get; set; }
    public bool CreditAllowed { get; set; }
    public Guid? PriceListId { get; set; }

    public PriceList? PriceList { get; set; }
}
