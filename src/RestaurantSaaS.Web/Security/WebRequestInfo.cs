using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Web.Security;

/// <summary>The caller's address for API calls (it is written to the login history). Calls from the Blazor pages have none.</summary>
internal sealed class WebRequestInfo(IHttpContextAccessor http) : IRequestInfo
{
    public string? IpAddress => http.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
