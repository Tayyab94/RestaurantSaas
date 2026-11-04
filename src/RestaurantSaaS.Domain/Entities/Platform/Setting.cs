using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Setting: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class Setting : TenantEntity
{
    public Guid? BranchId { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string ValueType { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }

    public Branch? Branch { get; set; }
}
