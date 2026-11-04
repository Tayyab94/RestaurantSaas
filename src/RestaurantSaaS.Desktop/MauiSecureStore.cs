using Microsoft.Maui.Storage;
using RestaurantSaaS.Shared.UI.Services.Auth;

namespace RestaurantSaaS.Desktop;

/// <summary>Keeps the refresh token in the operating system's secure storage.</summary>
internal sealed class MauiSecureStore : ISecureStore
{
    public async ValueTask<string?> GetAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key);
        }
        catch (Exception)
        {
            // Storage unavailable or damaged: behave as "not signed in".
            return null;
        }
    }

    public async ValueTask SetAsync(string key, string value) => await SecureStorage.Default.SetAsync(key, value);

    public ValueTask RemoveAsync(string key)
    {
        SecureStorage.Default.Remove(key);
        return ValueTask.CompletedTask;
    }
}
