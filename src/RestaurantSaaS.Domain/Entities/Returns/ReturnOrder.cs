using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ReturnOrder: branch table. Part of the Returns module. See Database Schema.</summary>
public sealed class ReturnOrder : BranchEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public ReturnOrderReturnType ReturnType { get; set; }
    public Guid ReasonId { get; set; }
    public string? ReasonNote { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public ReturnOrderRefundMethod RefundMethod { get; set; }
    public ReturnStatus Status { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? CreditNoteInvoiceId { get; set; }
    public Guid? ShiftId { get; set; }
    public DateOnly BusinessDate { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public Order? Order { get; set; }
    public Customer? Customer { get; set; }
    public ReturnReason Reason { get; set; } = null!;
    public Invoice? CreditNoteInvoice { get; set; }
    public Shift? Shift { get; set; }
}
