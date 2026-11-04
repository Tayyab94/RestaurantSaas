using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Contracts.Services;

public interface IAuthService
{
    Task<AuthTokensDto> LoginAsync(LoginRequest request, CancellationToken ct = default);

    Task<RefreshResultDto> RefreshAsync(RefreshRequest request, CancellationToken ct = default);

    Task LogoutAsync(LogoutRequest request, CancellationToken ct = default);

    Task<RegisterBusinessResultDto> RegisterBusinessAsync(RegisterBusinessRequest request, CancellationToken ct = default);

    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default);

    /// <summary>Quick sign-in for staff on a shared screen: business code, username and a 4 to 6 digit PIN.</summary>
    Task<AuthTokensDto> PinLoginAsync(PinLoginRequest request, CancellationToken ct = default);

    /// <summary>Sends a code to the phone or email on file. The answer is always the same, so nobody can find out which accounts exist.</summary>
    Task RequestPasswordResetAsync(RequestPasswordResetRequest request, CancellationToken ct = default);

    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
}
