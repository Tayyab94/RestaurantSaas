using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Auth;

/// <summary>
/// The signed-in session of one UI instance (one browser circuit, or the desktop window).
/// Holds the user, the access token and the refresh token; restores the session on start from the secure store.
/// </summary>
public sealed class AuthSession(IAuthService auth, ISecureStore store, AuthSessionOptions options) : ISessionAccessor
{
    private const string RefreshKey = "restopos.refresh";

    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _initialized;
    private string? _accessToken;
    private DateTime _accessExpiresAtUtc;
    private string? _refreshToken;

    public CurrentUserDto? User { get; private set; }

    public bool IsAuthenticated => User is not null;

    /// <summary>Raised when the user signs in, signs out or the session expires.</summary>
    public event Action? Changed;

    /// <summary>True when the signed-in user has the permission (the SuperAdmin has all).</summary>
    public bool Has(string permission) => User is { } user && (user.IsSuperAdmin || user.Permissions.Contains(permission));

    /// <summary>Restores the session from the stored refresh token. Safe to call many times.</summary>
    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _gate.WaitAsync();

        try
        {
            if (_initialized)
            {
                return;
            }

            if (!options.RestoreSession)
            {
                // This host asks for the sign-in every time it starts (the desktop app).
                await SafeRemoveAsync();
                _initialized = true;
                return;
            }

            string? refresh;

            try
            {
                refresh = await store.GetAsync(RefreshKey);
            }
            catch (InvalidOperationException)
            {
                // Browser storage is not available yet (before the first render). Try again later.
                return;
            }

            var restored = false;

            if (refresh is not null)
            {
                try
                {
                    var result = await auth.RefreshAsync(new RefreshRequest(refresh));
                    Apply(result.AccessToken, result.AccessTokenExpiresAtUtc, result.User, refresh);
                    restored = true;
                }
                catch (ServiceException)
                {
                    await SafeRemoveAsync();
                }
            }

            _initialized = true;

            if (restored)
            {
                // Tell the menu and the layout that a user is signed in now.
                Changed?.Invoke();
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public Task SignInAsync(LoginRequest request) => AcceptAsync(auth.LoginAsync(request));

    public Task SignInWithPinAsync(PinLoginRequest request) => AcceptAsync(auth.PinLoginAsync(request));

    private async Task AcceptAsync(Task<AuthTokensDto> signIn)
    {
        var tokens = await signIn;
        Apply(tokens.AccessToken, tokens.AccessTokenExpiresAtUtc, tokens.User, tokens.RefreshToken);
        _initialized = true;

        try
        {
            await store.SetAsync(RefreshKey, tokens.RefreshToken);
        }
        catch (InvalidOperationException)
        {
            // The session still works for this visit; it just cannot be restored later.
        }

        Changed?.Invoke();
    }

    public async Task SignOutAsync()
    {
        var refresh = _refreshToken;
        Clear();
        await SafeRemoveAsync();
        Changed?.Invoke();

        if (refresh is not null)
        {
            try
            {
                await auth.LogoutAsync(new LogoutRequest(refresh));
            }
            catch (ServiceException)
            {
                // The token expires by itself; nothing more to do.
            }
        }
    }

    /// <summary>
    /// Reloads the user's roles and permissions, so a change made by the Owner applies within minutes, and signs out
    /// a user who was disabled or whose business was suspended. Network trouble is ignored (it is tried again later).
    /// </summary>
    public async Task RefreshProfileAsync()
    {
        var refresh = _refreshToken;

        if (refresh is null)
        {
            return;
        }

        try
        {
            var result = await auth.RefreshAsync(new RefreshRequest(refresh));
            var before = User;
            Apply(result.AccessToken, result.AccessTokenExpiresAtUtc, result.User, refresh);

            if (before is null
                || !before.Permissions.SequenceEqual(result.User.Permissions)
                || !before.Roles.SequenceEqual(result.User.Roles)
                || before.Branches.Count != result.User.Branches.Count)
            {
                Changed?.Invoke();
            }
        }
        catch (ServiceException ex) when (ex.Code is "invalid_refresh_token" or "tenant_suspended" or "unauthenticated")
        {
            Clear();
            Changed?.Invoke();
        }
        catch (ServiceException)
        {
            // Not reachable right now; the next check tries again.
        }
    }

    /// <summary>A valid access token for API calls. Renews it with the refresh token shortly before it expires.</summary>
    public async ValueTask<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (_accessToken is null || _refreshToken is null)
        {
            return null;
        }

        if (_accessExpiresAtUtc > DateTime.UtcNow.AddSeconds(30))
        {
            return _accessToken;
        }

        await _gate.WaitAsync(ct);

        try
        {
            if (_accessExpiresAtUtc > DateTime.UtcNow.AddSeconds(30))
            {
                return _accessToken;
            }

            var result = await auth.RefreshAsync(new RefreshRequest(_refreshToken!), ct);
            Apply(result.AccessToken, result.AccessTokenExpiresAtUtc, result.User, _refreshToken!);
            return _accessToken;
        }
        catch (ServiceException)
        {
            Clear();
            Changed?.Invoke();
            return null;
        }
        finally
        {
            _gate.Release();
        }
    }

    private void Apply(string accessToken, DateTime expiresAtUtc, CurrentUserDto user, string refreshToken)
    {
        _accessToken = accessToken;
        _accessExpiresAtUtc = expiresAtUtc;
        _refreshToken = refreshToken;
        User = user;
    }

    private void Clear()
    {
        _accessToken = null;
        _refreshToken = null;
        User = null;
    }

    private async Task SafeRemoveAsync()
    {
        try
        {
            await store.RemoveAsync(RefreshKey);
        }
        catch (InvalidOperationException)
        {
            // Storage not available; nothing to remove.
        }
    }
}
