using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>GiftCard: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class GiftCard : TenantEntity
{
    public string CardNumber { get; set; } = string.Empty;
    public string? Pin { get; set; }
    public decimal InitialAmount { get; set; }
    public decimal Balance { get; set; }
    public Guid? IssuedToCustomerId { get; set; }
    public DateOnly? ExpiresOn { get; set; }
    public GiftCardStatus Status { get; set; }

    public Customer? IssuedToCustomer { get; set; }
}
