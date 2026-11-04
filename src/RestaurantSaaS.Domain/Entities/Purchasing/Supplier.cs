using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Supplier: tenant table. Part of the Purchasing module. See Database Schema.</summary>
public sealed class Supplier : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SupplierSupplierType SupplierType { get; set; }
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public int PaymentTermsDays { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AdvanceBalance { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsActive { get; set; }
    public decimal? Rating { get; set; }
}
