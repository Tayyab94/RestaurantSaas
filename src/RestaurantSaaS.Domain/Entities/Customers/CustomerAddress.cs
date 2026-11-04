using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CustomerAddress: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class CustomerAddress : TenantEntity
{
    public Guid CustomerId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public Guid? AreaId { get; set; }
    public string? Landmark { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; }

    public Customer Customer { get; set; } = null!;
    public Area? Area { get; set; }
}
