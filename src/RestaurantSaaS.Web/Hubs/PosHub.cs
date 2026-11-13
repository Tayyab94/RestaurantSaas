using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RestaurantSaaS.Application.Abstractions;   // ADD
namespace RestaurantSaaS.Web.Hubs;

/// <summary>What the server can push to clients (kitchen, counter, waiters). Grows with module M13.</summary>
public interface IPosHubClient
{
    Task OrderCreated(Guid orderId);

    Task OrderReady(Guid orderId);

    Task ItemAvailabilityChanged(Guid menuItemId, bool isAvailable);
}

public static class HubGroups
{
    public static string Branch(Guid tenantId, Guid branchId) => $"t:{tenantId:N}:b:{branchId:N}";

    public static string Role(Guid tenantId, Guid branchId, string role) => $"{Branch(tenantId, branchId)}:{role}";

    public static string User(Guid tenantId, Guid branchId, Guid userId) => $"{Branch(tenantId, branchId)}:user:{userId:N}";
}

/// <summary>
/// Real-time hub. Only signed-in users can connect. Joining groups (bound to the user's tenant and branch)
/// is added with module M13.
/// </summary>
[Authorize]
public sealed class PosHub(ILicenseGate licenseGate) : Hub<IPosHubClient>   // CHANGE
{
    public override async Task OnConnectedAsync()
    {
        try
        {
            await licenseGate.EnsureCanSellAsync(Context.ConnectionAborted);
        }
        catch (RestaurantSaaS.Application.Common.Exceptions.LicenseException ex)
        {
            throw new HubException(ex.Message);
        }

        await base.OnConnectedAsync();
    }
}
