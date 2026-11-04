using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>A named set of permissions inside one tenant. System roles are created for every new tenant.</summary>
public sealed class Role : TenantEntity
{
    private Role()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsSystem { get; private set; }

    /// <summary>Changes the name and description of a custom role. System roles keep their name.</summary>
    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Description = description;
    }

    public static Role Create(Guid tenantId, string name, string? description, bool isSystem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Role
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Description = description,
            IsSystem = isSystem,
        };
    }
}
