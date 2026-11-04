namespace RestaurantSaaS.Shared.UI.Services.Auth;

/// <summary>Where the refresh token is kept between visits. Web: encrypted browser storage. Desktop: the OS secure store.</summary>
public interface ISecureStore
{
    ValueTask<string?> GetAsync(string key);

    ValueTask SetAsync(string key, string value);

    ValueTask RemoveAsync(string key);
}
