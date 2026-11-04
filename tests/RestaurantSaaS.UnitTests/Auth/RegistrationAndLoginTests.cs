using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Domain.Security;
using RestaurantSaaS.Infrastructure.Persistence;
using RestaurantSaaS.Infrastructure.Services;

namespace RestaurantSaaS.UnitTests.Auth;

/// <summary>Registration and sign-in on the real handlers, the real password hasher and an in-memory database.</summary>
public class RegistrationAndLoginTests
{
    private const string Password = "Passw0rd!x";

    private readonly TestDatabase _database = new();
    private readonly Pbkdf2PasswordHasher _hasher = new();

    private async Task<(AppDbContext Db, ProvisionedTenant Tenant)> RegisterAsync()
    {
        var db = _database.CreateContext();
        await DatabaseSeeder.EnsurePermissionsAsync(db);

        var provisioner = new TenantProvisioner(db, _hasher, _database.Clock);
        var tenant = await provisioner.ProvisionAsync(
            new ProvisionTenantRequest("Test Cafe", BusinessType.Cafe, DeploymentMode.Cloud, "Owner Name", "owner", Password));

        return (db, tenant);
    }

    private LoginHandler CreateLogin(AppDbContext db) =>
        new(new SignInService(db, _hasher, new FakeTokenService(), new UserSessionBuilder(db), _database.Clock, new NullRequestInfo()));

    [Fact]
    public async Task Registration_creates_the_tenant_its_roles_and_an_owner_with_all_business_permissions()
    {
        var (db, result) = await RegisterAsync();

        Assert.Equal(TenantStatus.Trial, result.Tenant.Status);
        Assert.Equal(_database.Clock.UtcNow.AddDays(14), result.Tenant.TrialEndsAt);
        Assert.Equal(SystemRoles.All.Count, await db.Roles.IgnoreQueryFilters().CountAsync(r => r.TenantId == result.Tenant.Id));
        Assert.Equal(1, await db.Branches.IgnoreQueryFilters().CountAsync(b => b.TenantId == result.Tenant.Id));

        var tokens = await CreateLogin(db).HandleAsync(new LoginCommand(result.Tenant.Code, "owner", Password), default);

        Assert.Contains(SystemRoles.Owner, tokens.User.Roles);
        Assert.Contains(Permissions.OrderCreate, tokens.User.Permissions);
        Assert.DoesNotContain(Permissions.TenantView, tokens.User.Permissions);
    }

    [Fact]
    public async Task Sign_in_is_not_case_sensitive_for_the_business_code_and_username()
    {
        var (db, result) = await RegisterAsync();

        var tokens = await CreateLogin(db).HandleAsync(new LoginCommand(result.Tenant.Code.ToLowerInvariant(), "OWNER", Password), default);

        Assert.Equal(result.Owner.Id, tokens.User.Id);
        Assert.NotEmpty(tokens.RefreshToken);
    }

    [Fact]
    public async Task A_wrong_password_a_wrong_code_and_an_unknown_user_give_the_same_error()
    {
        var (db, result) = await RegisterAsync();
        var login = CreateLogin(db);

        var wrongPassword = await Assert.ThrowsAsync<UnauthorizedException>(() => login.HandleAsync(new LoginCommand(result.Tenant.Code, "owner", "wrong"), default));
        var wrongCode = await Assert.ThrowsAsync<UnauthorizedException>(() => login.HandleAsync(new LoginCommand("NOPE99", "owner", Password), default));
        var unknownUser = await Assert.ThrowsAsync<UnauthorizedException>(() => login.HandleAsync(new LoginCommand(result.Tenant.Code, "ghost", Password), default));

        Assert.Equal("invalid_credentials", wrongPassword.Code);
        Assert.Equal(wrongPassword.Message, wrongCode.Message);
        Assert.Equal(wrongPassword.Message, unknownUser.Message);
    }

    [Fact]
    public async Task Five_wrong_passwords_lock_the_account()
    {
        var (db, result) = await RegisterAsync();
        var login = CreateLogin(db);

        for (var i = 0; i < 5; i++)
        {
            await Assert.ThrowsAsync<UnauthorizedException>(() => login.HandleAsync(new LoginCommand(result.Tenant.Code, "owner", "wrong"), default));
        }

        var locked = await Assert.ThrowsAsync<UnauthorizedException>(() => login.HandleAsync(new LoginCommand(result.Tenant.Code, "owner", Password), default));
        Assert.Equal("locked", locked.Code);

        _database.Clock.UtcNow = _database.Clock.UtcNow.AddMinutes(16);
        var tokens = await login.HandleAsync(new LoginCommand(result.Tenant.Code, "owner", Password), default);
        Assert.Equal(result.Owner.Id, tokens.User.Id);
    }

    [Fact]
    public async Task The_default_super_admin_signs_in_without_a_business_code_and_must_change_the_password()
    {
        await using var db = _database.CreateContext();
        await DatabaseSeeder.EnsurePermissionsAsync(db);
        await DatabaseSeeder.EnsureSuperAdminAsync(db, _hasher, new SeedOptions { SuperAdminPassword = "Super!Pass1" }, NullLogger.Instance);

        var tokens = await CreateLogin(db).HandleAsync(new LoginCommand(null, "superadmin", "Super!Pass1"), default);

        Assert.True(tokens.User.IsSuperAdmin);
        Assert.True(tokens.User.MustChangePassword);
        Assert.Contains(Permissions.TenantView, tokens.User.Permissions);
    }

    [Fact]
    public async Task No_super_admin_is_created_when_no_password_is_configured()
    {
        await using var db = _database.CreateContext();

        await DatabaseSeeder.EnsureSuperAdminAsync(db, _hasher, new SeedOptions(), NullLogger.Instance);

        Assert.Empty(await db.Users.ToListAsync());
    }

    [Fact]
    public async Task A_suspended_business_cannot_sign_in()
    {
        var (db, result) = await RegisterAsync();
        var tenant = await db.Tenants.IgnoreQueryFilters().SingleAsync(t => t.Id == result.Tenant.Id);
        typeof(RestaurantSaaS.Domain.Entities.Tenant).GetProperty(nameof(tenant.Status))!.SetValue(tenant, TenantStatus.Suspended);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<UnauthorizedException>(() => CreateLogin(db).HandleAsync(new LoginCommand(result.Tenant.Code, "owner", Password), default));

        Assert.Equal("tenant_suspended", ex.Code);
    }
}
