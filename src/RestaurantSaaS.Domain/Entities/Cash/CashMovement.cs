using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CashMovement: branch table. Part of the Cash module. See Database Schema.</summary>
public sealed class CashMovement : BranchEntity
{
    public Guid? ClientRequestId { get; set; }
    public Guid ShiftId { get; set; }
    public CashMovementMovementType MovementType { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid PerformedBy { get; set; }
    public Guid? ApprovedBy { get; set; }

    public Shift Shift { get; set; } = null!;
}
