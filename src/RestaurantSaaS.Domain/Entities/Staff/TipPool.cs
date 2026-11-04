using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TipPool: branch table. Part of the Staff module. See Database Schema.</summary>
public sealed class TipPool : BranchEntity
{
    public DateOnly BusinessDate { get; set; }
    public decimal TotalTips { get; set; }
    public TipPoolMethod Method { get; set; }
    public TipPoolStatus Status { get; set; }
    public Guid? ApprovedBy { get; set; }
}
