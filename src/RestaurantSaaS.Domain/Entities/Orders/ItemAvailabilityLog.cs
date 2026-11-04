using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ItemAvailabilityLog: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class ItemAvailabilityLog : BranchEntity
{
    public Guid MenuItemId { get; set; }
    public Guid? VariantId { get; set; }
    public ItemAvailabilityLogAction Action { get; set; }
    public ItemAvailabilitySource Source { get; set; }
    public string? Reason { get; set; }
    public DateTime? ExpectedBackAt { get; set; }
    public Guid? UserId { get; set; }
    public Guid? DeviceId { get; set; }
    public DateTime OccurredAt { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public ItemVariant? Variant { get; set; }
    public User? User { get; set; }
    public Device? Device { get; set; }
}
