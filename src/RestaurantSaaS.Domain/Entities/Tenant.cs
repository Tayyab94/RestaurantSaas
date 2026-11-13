using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>
/// One customer business of the SaaS (a restaurant or a chain). Every tenant has its own branches, users,
/// menu and orders, and can never see another tenant's data.
/// </summary>
public sealed class Tenant : BaseEntity
{
    private Tenant()
    {
    }

    /// <summary>Short business code typed at sign-in (for example PIZZA42).</summary>
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public BusinessType BusinessType { get; private set; }

    public string OwnerName { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public TenantStatus Status { get; private set; }

    public DeploymentMode DeploymentMode { get; private set; }

    public DateTime? TrialEndsAt { get; private set; }

    public string CurrencyCode { get; private set; } = "PKR";

    public string TimeZone { get; private set; } = "Asia/Karachi";

    public string DefaultLanguage { get; private set; } = "en";

    public static Tenant Create(
        string code,
        string name,
        string ownerName,
        BusinessType businessType,
        DeploymentMode deploymentMode,
        Guid? id = null,
        DateTime? trialEndsAt = null,
        string? email = null,
        string? phone = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerName);

        var tenant = new Tenant
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            OwnerName = ownerName.Trim(),
            BusinessType = businessType,
            DeploymentMode = deploymentMode,
            Status = TenantStatus.Trial,
            TrialEndsAt = trialEndsAt,
            Email = email?.Trim().ToLowerInvariant(),
            Phone = phone?.Trim(),
        };

        if (id.HasValue)
        {
            tenant.Id = id.Value;
        }

        return tenant;
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }


    /// <summary>A paid license or subscription was activated: the business leaves the trial.</summary>
    public void Activate()
    {
        if (Status == TenantStatus.Trial)
        {
            Status = TenantStatus.Active;
        }
    }

    public bool CanSignIn => Status is TenantStatus.Trial or TenantStatus.Active;
}
