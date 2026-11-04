using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>CashDrawerLog: branch table. Part of the Cash module. See Database Schema.</summary>
public sealed class CashDrawerLog : BranchEntity
{
    public Guid ShiftId { get; set; }
    public Guid OpenedBy { get; set; }
    public DateTime OpenedAt { get; set; }
    public CashDrawerReason Reason { get; set; }

    public Shift Shift { get; set; } = null!;
}
