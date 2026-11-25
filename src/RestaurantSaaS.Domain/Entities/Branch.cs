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

    /// <summary>Changes the details of the outlet. The code stays as it was created.</summary>
    public void Update(string name, string? city, string? address, string? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
    }
}
