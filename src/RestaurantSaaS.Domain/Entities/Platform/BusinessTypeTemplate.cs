using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>BusinessTypeTemplate: platform table (no tenant). Part of the Platform module. See Database Schema.</summary>
public sealed class BusinessTypeTemplate : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TerminologyJson { get; set; }
    public string? DefaultSettingsJson { get; set; }
}
