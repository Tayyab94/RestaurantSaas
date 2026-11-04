using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>KdsScreen: branch table. Part of the Orders module. See Database Schema.</summary>
public sealed class KdsScreen : BranchEntity
{
    public Guid DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public KdsScreenScreenType ScreenType { get; set; }
    public string? StationIdsJson { get; set; }
    public bool ShowReady { get; set; }
    public int WarnAfterMinutes { get; set; }
    public int LateAfterMinutes { get; set; }
    public bool SoundEnabled { get; set; }
    public bool ShowAllergens { get; set; }
    public bool PriorityFirst { get; set; }
    public string? Theme { get; set; }
    public bool IsActive { get; set; }

    public Device Device { get; set; } = null!;
}
