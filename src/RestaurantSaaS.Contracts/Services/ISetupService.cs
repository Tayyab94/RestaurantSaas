using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

public interface ISetupService
{
    /// <summary>Open to everyone: tells the sign-in screen whether this installation still needs its first-run setup.</summary>
    Task<SetupStatusDto> GetStatusAsync(CancellationToken ct = default);
}
