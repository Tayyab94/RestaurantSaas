using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ConsolidatedInvoice: branch table. Part of the Customers module. See Database Schema.</summary>
public sealed class ConsolidatedInvoice : BranchEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public DateOnly DueDate { get; set; }
    public ConsolidatedInvoiceStatus Status { get; set; }
    public DateTime? IssuedAt { get; set; }
    public Guid? IssuedBy { get; set; }

    public Customer Customer { get; set; } = null!;
}
