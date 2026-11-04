using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Infrastructure.Persistence;
using RestaurantSaaS.Infrastructure.Persistence.Interceptors;

namespace RestaurantSaaS.UnitTests;

internal sealed class FixedClock(DateTime now) : IClock
{
    public DateTime UtcNow { get; set; } = now;
}

internal sealed class TestTenantProvider(Guid? tenantId) : ITenantProvider
{
    public Guid? TenantId => tenantId;
}

internal sealed class TestCurrentUser(bool authenticated = true, bool superAdmin = false, string[]? permissions = null) : ICurrentUser
{
    public bool IsAuthenticated => authenticated;

    public Guid? UserId => authenticated ? Guid.Parse("00000000-0000-0000-0000-0000000000aa") : null;

    public string? UserName => authenticated ? "tester" : null;

    public Guid? TenantId => null;

    public bool IsSuperAdmin => superAdmin;

    public bool HasPermission(string permission) => superAdmin || (permissions?.Contains(permission) ?? false);
}

internal sealed class FakeTokenService : ITokenService
{
    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(CurrentUserDto user) =>
        ($"token-for-{user.UserName}", DateTime.UtcNow.AddMinutes(15));
}

/// <summary>An in-memory database shared by all contexts of one test, with the real audit interceptor.</summary>
internal sealed class TestDatabase
{
    private readonly string _name = Guid.NewGuid().ToString();

    public FixedClock Clock { get; } = new(new DateTime(2026, 10, 12, 9, 0, 0, DateTimeKind.Utc));

    public AppDbContext CreateContext(Guid? tenantId = null)
    {
        var provider = new TestTenantProvider(tenantId);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(_name)
            .AddInterceptors(new AuditingInterceptor(Clock, new TestCurrentUser(), provider))
            .Options;

        return new AppDbContext(options, provider);
    }
}
