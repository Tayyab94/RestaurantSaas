using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>TemperatureLog: branch table. Part of the FoodSafety module. See Database Schema.</summary>
public sealed class TemperatureLog : BranchEntity
{
    public Guid? EquipmentId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal TemperatureC { get; set; }
    public decimal MinC { get; set; }
    public decimal MaxC { get; set; }
    public bool IsInRange { get; set; }
    public TemperatureSource Source { get; set; }
    public Guid? RecordedBy { get; set; }
    public DateTime RecordedAt { get; set; }
    public string? CorrectiveAction { get; set; }
    public Guid? IncidentId { get; set; }

    public Equipment? Equipment { get; set; }
    public StockLocation? Location { get; set; }
    public FoodSafetyIncident? Incident { get; set; }
}
