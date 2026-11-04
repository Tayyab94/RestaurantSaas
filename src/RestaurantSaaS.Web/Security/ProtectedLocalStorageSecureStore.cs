using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RestaurantSaaS.Shared.UI.Services.Auth;

namespace RestaurantSaaS.Web.Security;

/// <summary>Keeps the refresh token in the browser, encrypted with ASP.NET Core data protection.</summary>
internal sealed class ProtectedLocalStorageSecureStore(ProtectedLocalStorage storage) : ISecureStore
{
    public async ValueTask<string?> GetAsync(string key)
    {
        var result = await storage.GetAsync<string>(key);
        return result.Success ? result.Value : null;
    }

    public ValueTask SetAsync(string key, string value) => storage.SetAsync(key, value);

    public ValueTask RemoveAsync(string key) => storage.DeleteAsync(key);
}
