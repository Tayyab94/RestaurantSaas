using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.Infrastructure.Persistence;

/// <summary>Fixed id of the demo business (development only).</summary>
public static class DevSeed
{
    public static readonly Guid TenantId = Guid.Parse("01950000-0000-7000-8000-000000000001");

    public const string DemoBusinessCode = "DEMO";
}

public static class DatabaseSeeder
{
    /// <summary>Adds any permission of the catalog (Permissions.All) that is not yet in the database.</summary>
    public static async Task EnsurePermissionsAsync(AppDbContext db, CancellationToken ct = default)
    {
        var existing = await db.Permissions.Select(p => p.Key).ToListAsync(ct);

        foreach (var info in Permissions.All.Where(p => !existing.Contains(p.Key)))
        {
            db.Permissions.Add(Permission.Create(info.Key, info.Module, info.Description));
        }

        await db.SaveChangesAsync(ct);
    }

    /// <summary>The Owner role of every business always holds every business permission, also those added after the business was created.</summary>
    public static async Task EnsureOwnerPermissionsAsync(AppDbContext db, CancellationToken ct = default)
    {
        var permissionIds = await db.Permissions
            .Where(p => p.Module != Permissions.PlatformModule)
            .Select(p => p.Id)
            .ToListAsync(ct);

        var ownerRoles = await db.Roles.IgnoreQueryFilters()
            .Where(r => !r.IsDeleted && r.IsSystem && r.Name == SystemRoles.Owner)
            .Select(r => new { r.Id, r.TenantId })
            .ToListAsync(ct);

        var ownerRoleIds = ownerRoles.Select(r => r.Id).ToList();

        var existing = (await db.RolePermissions.IgnoreQueryFilters()
                .Where(rp => !rp.IsDeleted && ownerRoleIds.Contains(rp.RoleId))
                .Select(rp => new { rp.RoleId, rp.PermissionId })
                .ToListAsync(ct))
            .Select(rp => (rp.RoleId, rp.PermissionId))
            .ToHashSet();

        foreach (var role in ownerRoles)
        {
            foreach (var permissionId in permissionIds.Where(id => !existing.Contains((role.Id, id))))
            {
                db.RolePermissions.Add(RolePermission.Create(role.TenantId, role.Id, permissionId));
            }
        }

        await db.SaveChangesAsync(ct);
    }

    /// <summary>Creates the default SuperAdmin once, only when a password is configured. It must change the password at first sign-in.</summary>
    public static async Task EnsureSuperAdminAsync(
        AppDbContext db,
        IPasswordHasher hasher,
        SeedOptions options,
        ILogger logger,
        CancellationToken ct = default)
    {
        if (await db.Users.IgnoreQueryFilters().AnyAsync(u => u.IsSuperAdmin, ct))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.SuperAdminPassword))
        {
            logger.LogWarning("No SuperAdmin exists and Seed:SuperAdminPassword is empty, so none was created.");
            return;
        }

        db.Users.Add(User.Create(
            tenantId: null,
            options.SuperAdminUserName,
            options.SuperAdminFullName,
            hasher.Hash(options.SuperAdminPassword),
            isSuperAdmin: true,
            mustChangePassword: true));

        await db.SaveChangesAsync(ct);
        logger.LogInformation("The default SuperAdmin '{UserName}' was created. It must change its password at first sign-in.", options.SuperAdminUserName);
    }

    /// <summary>Development only: a demo business with an Owner user, so the app can be used at once.</summary>
    public static async Task EnsureDemoTenantAsync(
        AppDbContext db,
        TenantProvisioner provisioner,
        SeedOptions options,
        ILogger logger,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(options.DemoOwnerPassword)
            || await db.Tenants.IgnoreQueryFilters().AnyAsync(t => t.Code == DevSeed.DemoBusinessCode, ct))
        {
            return;
        }

        await provisioner.ProvisionAsync(
            new ProvisionTenantRequest(
                "Demo Restaurant",
                BusinessType.Restaurant,
                DeploymentMode.Cloud,
                "Demo Owner",
                "owner",
                options.DemoOwnerPassword,
                FixedCode: DevSeed.DemoBusinessCode,
                TenantId: DevSeed.TenantId),
            ct);

        logger.LogInformation("The demo business '{Code}' was created with the Owner user 'owner'.", DevSeed.DemoBusinessCode);
    }
}
