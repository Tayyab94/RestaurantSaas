using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Customer: tenant table. Part of the Customers module. See Database Schema.</summary>
public sealed class Customer : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Phone2 { get; set; }
    public string? Email { get; set; }
    public Guid? GroupId { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? Notes { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AdvanceBalance { get; set; }
    public int LoyaltyPoints { get; set; }
    public bool IsBlocked { get; set; }
    public string? TaxNumber { get; set; }
    public DateTime? LastOrderAt { get; set; }
    public CustomerCustomerType CustomerType { get; set; }
    public string? CompanyName { get; set; }
    public CustomerBillingCycle BillingCycle { get; set; }
    public int? CreditDays { get; set; }
    public Guid? PriceListId { get; set; }
    public Guid? MergedIntoCustomerId { get; set; }

    public CustomerGroup? Group { get; set; }
    public PriceList? PriceList { get; set; }
    public Customer? MergedIntoCustomer { get; set; }
}
