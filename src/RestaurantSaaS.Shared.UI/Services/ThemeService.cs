using Microsoft.JSInterop;

namespace RestaurantSaaS.Shared.UI.Services;

/// <summary>Light and dark theme. The choice is kept in the browser or WebView (localStorage) and, for a signed-in user, on the server.</summary>
public sealed class ThemeService(IJSRuntime js)
{
    /// <summary>Raised after the theme was set (true = dark), so every screen can update its icon.</summary>
    public event Action<bool>? Changed;

    /// <summary>Applies the stored theme and returns true when it is dark.</summary>
    public async ValueTask<bool> InitializeAsync() => await js.InvokeAsync<string>("rsTheme.get") == "dark";

    public async ValueTask SetDarkAsync(bool dark)
    {
        await js.InvokeVoidAsync("rsTheme.set", dark ? "dark" : "light");
        Changed?.Invoke(dark);
    }
}
