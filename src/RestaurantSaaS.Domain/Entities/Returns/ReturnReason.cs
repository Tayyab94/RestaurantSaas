using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>ReturnReason: tenant table. Part of the Returns module. See Database Schema.</summary>
public sealed class ReturnReason : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public bool RequiresNote { get; set; }
    public bool IsActive { get; set; }
}
