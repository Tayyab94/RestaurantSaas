using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Expense: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class Expense : BranchEntity
{
    public string ExpenseNumber { get; set; } = string.Empty;
    public Guid? ClientRequestId { get; set; }
    public Guid CategoryId { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string? PaidTo { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? ShiftId { get; set; }
    public string? Description { get; set; }
    public Guid? AttachmentId { get; set; }
    public Guid? ApprovedBy { get; set; }

    public ExpenseCategory Category { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public Shift? Shift { get; set; }
    public Attachment? Attachment { get; set; }
}
