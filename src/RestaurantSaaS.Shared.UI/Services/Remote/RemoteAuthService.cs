using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

internal sealed class RemoteAuthService(RemoteApiClient api) : IAuthService
{
    public Task<AuthTokensDto> LoginAsync(LoginRequest request, CancellationToken ct = default) =>
        api.PostAsync<AuthTokensDto>("api/v1/auth/login", request, authenticated: false, ct);

    public Task<RefreshResultDto> RefreshAsync(RefreshRequest request, CancellationToken ct = default) =>
        api.PostAsync<RefreshResultDto>("api/v1/auth/refresh", request, authenticated: false, ct);

    public Task LogoutAsync(LogoutRequest request, CancellationToken ct = default) =>
        api.PostAsync("api/v1/auth/logout", request, authenticated: false, ct);

    public Task<RegisterBusinessResultDto> RegisterBusinessAsync(RegisterBusinessRequest request, CancellationToken ct = default) =>
        api.PostAsync<RegisterBusinessResultDto>("api/v1/auth/register", request, authenticated: false, ct);

    public Task<AuthTokensDto> PinLoginAsync(PinLoginRequest request, CancellationToken ct = default) =>
        api.PostAsync<AuthTokensDto>("api/v1/auth/pin-login", request, authenticated: false, ct);

    public Task RequestPasswordResetAsync(RequestPasswordResetRequest request, CancellationToken ct = default) =>
        api.PostAsync("api/v1/auth/forgot-password", request, authenticated: false, ct);

    public Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default) =>
        api.PostAsync("api/v1/auth/reset-password", request, authenticated: false, ct);

    public Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default) =>
        api.PostAsync("api/v1/account/change-password", request, authenticated: true, ct);
}
