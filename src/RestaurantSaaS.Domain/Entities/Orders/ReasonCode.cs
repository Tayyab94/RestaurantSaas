using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ReasonCode: tenant table. Part of the Orders module. See Database Schema.</summary>
public sealed class ReasonCode : TenantEntity
{
    public ReasonCodeReasonType ReasonType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameUrdu { get; set; }
    public bool RequiresNote { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
