using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>One physical outlet of a tenant.</summary>
public sealed class Branch : TenantEntity
{
    private Branch()
    {
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? City { get; private set; }

    public string? Address { get; private set; }

    public string? Phone { get; private set; }

    public bool IsMain { get; private set; }

    public static Branch Create(Guid tenantId, string code, string name, bool isMain, Guid? id = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var branch = new Branch
        {
            TenantId = tenantId,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            IsMain = isMain,
        };

        if (id.HasValue)
        {
            branch.Id = id.Value;
        }

        return branch;
    }
}
