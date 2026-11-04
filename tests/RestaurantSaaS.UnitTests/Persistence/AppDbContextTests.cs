using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Domain.Entities;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.UnitTests.Persistence;

/// <summary>
/// Data-layer rules: audit columns, soft delete and tenant isolation.
/// These run on the in-memory provider; unique-index behaviour is checked in integration tests on the real databases.
/// </summary>
public class AppDbContextTests
{
    private static readonly Guid TenantA = Guid.CreateVersion7();
    private static readonly Guid TenantB = Guid.CreateVersion7();

    private readonly TestDatabase _database = new();

    private async Task SeedTenantsAsync()
    {
        await using var seed = _database.CreateContext();
        seed.Tenants.Add(Tenant.Create("A", "Tenant A", "Owner A", BusinessType.Restaurant, DeploymentMode.Cloud, TenantA));
        seed.Tenants.Add(Tenant.Create("B", "Tenant B", "Owner B", BusinessType.Cafe, DeploymentMode.Cloud, TenantB));
        await seed.SaveChangesAsync();
    }

    [Fact]
    public async Task New_rows_get_audit_columns()
    {
        await using var context = _database.CreateContext();
        var tenant = Tenant.Create("AUD", "Audit Test", "Owner", BusinessType.Bakery, DeploymentMode.OnPremise);

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        Assert.Equal(_database.Clock.UtcNow, tenant.CreatedAt);
        Assert.Equal(tenant.CreatedAt, tenant.UpdatedAt);
        Assert.False(tenant.IsDeleted);
    }

    [Fact]
    public async Task Queries_return_only_the_current_tenants_rows()
    {
        await SeedTenantsAsync();

        await using (var a = _database.CreateContext(TenantA))
        {
            a.Branches.Add(Branch.Create(TenantA, "MAIN", "A main", isMain: true));
            await a.SaveChangesAsync();
        }

        await using (var b = _database.CreateContext(TenantB))
        {
            b.Branches.Add(Branch.Create(TenantB, "MAIN", "B main", isMain: true));
            await b.SaveChangesAsync();
        }

        await using var readA = _database.CreateContext(TenantA);
        var names = await readA.Branches.Select(x => x.Name).ToListAsync();

        Assert.Single(names);
        Assert.Equal("A main", names[0]);
    }

    [Fact]
    public async Task Without_a_tenant_no_tenant_rows_are_visible()
    {
        await SeedTenantsAsync();

        await using (var a = _database.CreateContext(TenantA))
        {
            a.Branches.Add(Branch.Create(TenantA, "MAIN", "A main", isMain: true));
            await a.SaveChangesAsync();
        }

        await using var noTenant = _database.CreateContext();

        Assert.Empty(await noTenant.Branches.ToListAsync());
    }

    [Fact]
    public async Task Saving_a_row_of_another_tenant_is_refused()
    {
        await SeedTenantsAsync();

        await using var a = _database.CreateContext(TenantA);
        a.Branches.Add(Branch.Create(TenantB, "MAIN", "Foreign", isMain: true));

        await Assert.ThrowsAsync<InvalidOperationException>(() => a.SaveChangesAsync());
    }

    [Fact]
    public async Task Delete_is_a_soft_delete()
    {
        await SeedTenantsAsync();

        await using (var a = _database.CreateContext(TenantA))
        {
            a.Branches.Add(Branch.Create(TenantA, "MAIN", "A main", isMain: true));
            await a.SaveChangesAsync();
        }

        await using (var a = _database.CreateContext(TenantA))
        {
            a.Branches.Remove(await a.Branches.SingleAsync());
            await a.SaveChangesAsync();
        }

        await using var read = _database.CreateContext(TenantA);

        Assert.Empty(await read.Branches.ToListAsync());

        var row = Assert.Single(await read.Branches.IgnoreQueryFilters().ToListAsync());
        Assert.True(row.IsDeleted);
        Assert.NotNull(row.DeletedAt);
    }

    [Fact]
    public async Task Platform_users_and_tenant_users_are_separated_by_the_tenant_filter()
    {
        await SeedTenantsAsync();

        await using (var platform = _database.CreateContext())
        {
            platform.Users.Add(User.Create(null, "superadmin", "Super", "hash", isSuperAdmin: true));
            await platform.SaveChangesAsync();
        }

        await using (var a = _database.CreateContext(TenantA))
        {
            a.Users.Add(User.Create(TenantA, "owner", "Owner A", "hash"));
            await a.SaveChangesAsync();
        }

        await using var asPlatform = _database.CreateContext();
        await using var asTenantA = _database.CreateContext(TenantA);

        Assert.Equal("superadmin", Assert.Single(await asPlatform.Users.ToListAsync()).UserName);
        Assert.Equal("owner", Assert.Single(await asTenantA.Users.ToListAsync()).UserName);
    }
}
