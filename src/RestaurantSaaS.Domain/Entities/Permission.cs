using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>One thing a user may do (for example Order.Void). The catalog lives in code (Permissions) and is synced to this table.</summary>
public sealed class Permission : BaseEntity
{
    private Permission()
    {
    }

    public string Key { get; private set; } = string.Empty;

    public string Module { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public static Permission Create(string key, string module, string description) =>
        new() { Key = key, Module = module, Description = description };
}
