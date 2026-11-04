using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>DeliveryArea: branch table. Part of the Delivery module. See Database Schema.</summary>
public sealed class DeliveryArea : BranchEntity
{
    public Guid AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DeliveryCharge { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? EstimatedMinutes { get; set; }
    public bool IsActive { get; set; }

    public Area Area { get; set; } = null!;
}
