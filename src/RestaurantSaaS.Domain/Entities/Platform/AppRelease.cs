using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>AppRelease: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class AppRelease : BaseEntity
{
    public string Version { get; set; } = string.Empty;
    public AppReleaseChannel Channel { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? MinSupportedVersion { get; set; }
    public bool IsMandatory { get; set; }
    public string PackageUrl { get; set; } = string.Empty;
    public string PackageHash { get; set; } = string.Empty;
    public string? ReleaseNotes { get; set; }
    public bool IsActive { get; set; }
}
