using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>AccountMapping: tenant table. Part of the Accounting module. See Database Schema.</summary>
public sealed class AccountMapping : TenantEntity
{
    public AccountMappingEventType EventType { get; set; }
    public Guid? PaymentMethodId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid DebitAccountId { get; set; }
    public Guid CreditAccountId { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }
    public Branch? Branch { get; set; }
    public ChartOfAccount DebitAccount { get; set; } = null!;
    public ChartOfAccount CreditAccount { get; set; } = null!;
}
