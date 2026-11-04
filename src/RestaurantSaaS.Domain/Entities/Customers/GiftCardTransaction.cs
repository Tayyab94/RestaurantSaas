using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>GiftCardTransaction: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class GiftCardTransaction : TenantEntity
{
    public Guid GiftCardId { get; set; }
    public decimal Amount { get; set; }
    public GiftCardTransactionType Type { get; set; }
    public Guid? OrderId { get; set; }
    public Guid BranchId { get; set; }

    public GiftCard GiftCard { get; set; } = null!;
    public Order? Order { get; set; }
    public Branch Branch { get; set; } = null!;
}
