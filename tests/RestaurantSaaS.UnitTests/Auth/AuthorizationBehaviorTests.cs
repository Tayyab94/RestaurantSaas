using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Application.Messaging;
using RestaurantSaaS.Application.Security;

namespace RestaurantSaaS.UnitTests.Auth;

public class AuthorizationBehaviorTests
{
    private sealed record OpenRequest : IRequest<int>;

    [AllowAnonymousRequest]
    private sealed record AnonymousRequest : IRequest<int>;

    [RequirePermission("Order.Void")]
    private sealed record VoidRequest : IRequest<int>;

    private static Task<int> Next() => Task.FromResult(1);

    [Fact]
    public async Task Anonymous_requests_run_without_a_user()
    {
        var behavior = new AuthorizationBehavior<AnonymousRequest, int>(new TestCurrentUser(authenticated: false));

        Assert.Equal(1, await behavior.HandleAsync(new AnonymousRequest(), Next, default));
    }

    [Fact]
    public async Task Other_requests_need_a_signed_in_user()
    {
        var behavior = new AuthorizationBehavior<OpenRequest, int>(new TestCurrentUser(authenticated: false));

        var ex = await Assert.ThrowsAsync<UnauthorizedException>(() => behavior.HandleAsync(new OpenRequest(), Next, default));
        Assert.Equal(401, ex.StatusCode);
    }

    [Fact]
    public async Task A_missing_permission_is_forbidden()
    {
        var behavior = new AuthorizationBehavior<VoidRequest, int>(new TestCurrentUser(permissions: ["Order.Create"]));

        var ex = await Assert.ThrowsAsync<ForbiddenException>(() => behavior.HandleAsync(new VoidRequest(), Next, default));
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task The_right_permission_and_the_super_admin_pass()
    {
        var allowed = new AuthorizationBehavior<VoidRequest, int>(new TestCurrentUser(permissions: ["Order.Void"]));
        var superAdmin = new AuthorizationBehavior<VoidRequest, int>(new TestCurrentUser(superAdmin: true));

        Assert.Equal(1, await allowed.HandleAsync(new VoidRequest(), Next, default));
        Assert.Equal(1, await superAdmin.HandleAsync(new VoidRequest(), Next, default));
    }
}
