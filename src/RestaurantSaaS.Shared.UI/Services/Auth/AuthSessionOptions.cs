namespace RestaurantSaaS.Shared.UI.Services.Auth;

/// <summary>
/// How a host treats the stored sign-in. The web restores it (a browser refresh keeps you signed in).
/// The desktop app turns it off so the login page always comes first when the app starts.
/// </summary>
public sealed class AuthSessionOptions
{
    public bool RestoreSession { get; set; } = true;

    /// <summary>Sign out after this many minutes without any mouse, keyboard or touch activity. 0 turns it off.</summary>
    public int IdleTimeoutMinutes { get; set; } = 30;
}
