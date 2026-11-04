using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>LoyaltyTransaction: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class LoyaltyTransaction : TenantEntity
{
    public Guid? ProgramId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public int Points { get; set; }
    public LoyaltyTransactionType Type { get; set; }
    public int BalanceAfter { get; set; }

    public LoyaltyProgram? Program { get; set; }
    public Customer Customer { get; set; } = null!;
    public Order? Order { get; set; }
}
