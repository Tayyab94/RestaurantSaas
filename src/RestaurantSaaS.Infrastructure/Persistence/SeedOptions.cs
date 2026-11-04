namespace RestaurantSaaS.Infrastructure.Persistence;

/// <summary>Bound from the "Seed" section. Passwords are empty by default: nothing is created without them.</summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public string SuperAdminUserName { get; set; } = "superadmin";

    public string SuperAdminFullName { get; set; } = "Platform Administrator";

    /// <summary>When empty, no SuperAdmin is created. Set it with an environment variable in production.</summary>
    public string SuperAdminPassword { get; set; } = string.Empty;

    /// <summary>Development only: creates the DEMO business with an Owner user.</summary>
    public bool DemoData { get; set; }

    public string DemoOwnerPassword { get; set; } = string.Empty;
}
