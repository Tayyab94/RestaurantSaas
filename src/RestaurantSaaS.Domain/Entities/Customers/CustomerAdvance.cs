using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CustomerAdvance: branch table. Part of the Customers module. See Database Schema.</summary>
public sealed class CustomerAdvance : BranchEntity
{
    public Guid? ClientRequestId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public decimal UsedAmount { get; set; }
    public CustomerAdvancePurpose Purpose { get; set; }
    public DateOnly? EventDate { get; set; }
    public CustomerAdvanceStatus Status { get; set; }
    public Guid? PaymentId { get; set; }

    public Customer Customer { get; set; } = null!;
    public Payment? Payment { get; set; }
}
