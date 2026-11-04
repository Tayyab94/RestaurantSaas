using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Equipment: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class Equipment : BranchEntity
{
    public string Name { get; set; } = string.Empty;
    public EquipmentEquipmentType EquipmentType { get; set; }
    public Guid? LocationId { get; set; }
    public decimal? MinTempC { get; set; }
    public decimal? MaxTempC { get; set; }
    public Guid? SensorId { get; set; }
    public bool IsActive { get; set; }

    public StockLocation? Location { get; set; }
}
