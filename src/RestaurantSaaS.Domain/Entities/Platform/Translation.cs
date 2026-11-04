using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Translation: tenant table. Part of the Platform module. See Database Schema.</summary>
public sealed class Translation : TenantEntity
{
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
