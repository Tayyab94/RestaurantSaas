using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Shared.UI.Services.Auth;

/// <summary>Tells Blazor who is signed in, based on the AuthSession. Works the same in the browser and in the desktop app.</summary>
internal sealed class AppAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthSession _session;

    public AppAuthenticationStateProvider(AuthSession session)
    {
        _session = session;
        _session.Changed += OnSessionChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _session.InitializeAsync();
        return new AuthenticationState(CreatePrincipal(_session.User));
    }

    public void Dispose() => _session.Changed -= OnSessionChanged;

    private void OnSessionChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static ClaimsPrincipal CreatePrincipal(CurrentUserDto? user)
    {
        if (user is null)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(permission => new Claim("perm", permission)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "RestoPOS"));
    }
}
