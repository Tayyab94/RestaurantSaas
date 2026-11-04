using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Attachment: tenant table. Part of the System module. See Database Schema.</summary>
public sealed class Attachment : TenantEntity
{
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageUrl { get; set; } = string.Empty;
}
