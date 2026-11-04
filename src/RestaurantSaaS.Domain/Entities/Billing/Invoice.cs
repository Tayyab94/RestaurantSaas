using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Invoice: branch table. Part of the Billing module. See Database Schema.</summary>
public sealed class Invoice : BranchEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public InvoiceInvoiceType InvoiceType { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? OriginalInvoiceId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateOnly BusinessDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? SplitLabel { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal DeliveryCharge { get; set; }
    public decimal RoundOff { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public Guid CashierId { get; set; }
    public Guid? ShiftId { get; set; }
    public DateTime IssuedAt { get; set; }
    public int PrintCount { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public string? FbrQrData { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public bool IsTrialMarked { get; set; }
    public Guid? ConsolidatedInvoiceId { get; set; }

    public Order? Order { get; set; }
    public Invoice? OriginalInvoice { get; set; }
    public Customer? Customer { get; set; }
    public User Cashier { get; set; } = null!;
    public Shift? Shift { get; set; }
    public ConsolidatedInvoice? ConsolidatedInvoice { get; set; }
}
