namespace RestaurantSaaS.Contracts.Dtos;

public sealed record LoginRequest(string? BusinessCode, string UserName, string Password);

public sealed record PinLoginRequest(string BusinessCode, string UserName, string Pin);

public sealed record RequestPasswordResetRequest(string BusinessCode, string UserName);

public sealed record ResetPasswordRequest(string BusinessCode, string UserName, string Code, string NewPassword);

public sealed record RefreshRequest(string RefreshToken);

public sealed record LogoutRequest(string RefreshToken);

public sealed record RegisterBusinessRequest(
    string BusinessName,
    string BusinessType,
    string OwnerFullName,
    string UserName,
    string? Email,
    string? Phone,
    string Password);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public sealed record CurrentUserDto(
    Guid Id,
    string UserName,
    string FullName,
    Guid? TenantId,
    string? TenantName,
    string? BusinessCode,
    bool IsSuperAdmin,
    bool MustChangePassword,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<BranchAccessDto> Branches);

/// <summary>A branch the signed-in user may work in.</summary>
public sealed record BranchAccessDto(Guid Id, string Name, bool IsDefault);

public sealed record AuthTokensDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    CurrentUserDto User);

public sealed record RefreshResultDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    CurrentUserDto User);

public sealed record RegisterBusinessResultDto(string BusinessCode, string UserName, DateTime TrialEndsAtUtc);
