using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;

namespace RestaurantSaaS.UnitTests.Domain;

public class DomainTests
{
    [Fact]
    public void Tenant_create_normalizes_the_code_and_starts_as_trial()
    {
        var tenant = Tenant.Create(" demo ", " Demo Restaurant ", "Owner", BusinessType.Restaurant, DeploymentMode.Cloud);

        Assert.Equal("DEMO", tenant.Code);
        Assert.Equal("Demo Restaurant", tenant.Name);
        Assert.Equal(TenantStatus.Trial, tenant.Status);
        Assert.True(tenant.CanSignIn);
        Assert.NotEqual(Guid.Empty, tenant.Id);
    }

    [Fact]
    public void User_is_locked_out_after_too_many_failures_and_unlocked_after_success()
    {
        var now = new DateTime(2026, 10, 12, 9, 0, 0, DateTimeKind.Utc);
        var user = User.Create(Guid.NewGuid(), "ali", "Ali", "hash");

        for (var i = 0; i < 5; i++)
        {
            user.RegisterFailedLogin(now, 5, TimeSpan.FromMinutes(15));
        }

        Assert.True(user.IsLockedOut(now.AddMinutes(1)));
        Assert.False(user.IsLockedOut(now.AddMinutes(16)));

        user.RegisterSuccessfulLogin(now.AddMinutes(20));
        Assert.False(user.IsLockedOut(now.AddMinutes(20)));
        Assert.Equal(0, user.FailedLoginCount);
    }

    [Fact]
    public void Permission_keys_are_unique()
    {
        var keys = Permissions.All.Select(p => p.Key).ToList();
        Assert.Equal(keys.Count, keys.Distinct().Count());
    }

    [Fact]
    public void Every_system_role_uses_only_catalog_permissions()
    {
        var catalog = Permissions.All.Select(p => p.Key).ToHashSet();

        foreach (var role in SystemRoles.All)
        {
            Assert.All(role.Permissions, key => Assert.Contains(key, catalog));
        }
    }

    [Fact]
    public void Tenant_roles_never_get_platform_permissions()
    {
        var platform = Permissions.All.Where(p => p.Module == Permissions.PlatformModule).Select(p => p.Key).ToList();

        foreach (var role in SystemRoles.All)
        {
            Assert.Empty(role.Permissions.Intersect(platform));
        }
    }
}
