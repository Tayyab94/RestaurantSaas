using RestaurantSaaS.Application.Features.Auth;
using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Contracts.Services;

namespace RestaurantSaaS.Application.Services;

internal sealed class LocalAuthService(LocalExecutor executor) : IAuthService
{
    public Task<AuthTokensDto> LoginAsync(LoginRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new LoginCommand(request.BusinessCode, request.UserName, request.Password), ct);

    public Task<AuthTokensDto> PinLoginAsync(PinLoginRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new PinLoginCommand(request.BusinessCode, request.UserName, request.Pin), ct);

    public Task<RefreshResultDto> RefreshAsync(RefreshRequest request, CancellationToken ct = default) =>
        executor.SendAsync(new RefreshTokenCommand(request.RefreshToken), ct);

    public async Task LogoutAsync(LogoutRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(new LogoutCommand(request.RefreshToken), ct);

    public Task<RegisterBusinessResultDto> RegisterBusinessAsync(RegisterBusinessRequest request, CancellationToken ct = default) =>
        executor.SendAsync(
            new RegisterBusinessCommand(request.BusinessName, request.BusinessType, request.OwnerFullName, request.UserName, request.Email, request.Phone, request.Password),
            ct);

    public async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), ct);

    public async Task RequestPasswordResetAsync(RequestPasswordResetRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(new RequestPasswordResetCommand(request.BusinessCode, request.UserName), ct);

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default) =>
        await executor.SendAsync(new ResetPasswordCommand(request.BusinessCode, request.UserName, request.Code, request.NewPassword), ct);
}
