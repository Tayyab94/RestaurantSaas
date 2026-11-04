namespace RestaurantSaaS.Shared.UI.Services;

/// <summary>
/// Decides where to go after sign-in. Only a relative address inside the app is accepted, so a crafted link
/// cannot send the user to another site. The sign-in and registration pages are never a target (no loops).
/// An empty result means the dashboard.
/// </summary>
public static class ReturnUrlHelper
{
    public static string Resolve(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return string.Empty;
        }

        var url = returnUrl.Trim();

        if (url.Contains("//", StringComparison.Ordinal)
            || url.Contains("\\", StringComparison.Ordinal)
            || url.Contains("://", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        url = url.TrimStart('/');
        var path = url.Split('?', '#')[0];

        if (path.Contains(':'))
        {
            return string.Empty;
        }

        var isAuthPage = path.Equals("login", StringComparison.OrdinalIgnoreCase)
                         || path.Equals("register", StringComparison.OrdinalIgnoreCase);

        return isAuthPage ? string.Empty : url;
    }
}
