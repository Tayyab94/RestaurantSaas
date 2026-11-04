namespace RestaurantSaaS.Contracts.Dtos;

public sealed record UserListItemDto(
    Guid Id,
    string UserName,
    string FullName,
    string? Email,
    bool IsActive,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles,
    bool HasPin,
    IReadOnlyList<string> BranchNames,
    IReadOnlyList<Guid> BranchIds,
    Guid? RoleId);

public sealed record RoleDto(Guid Id, string Name, string? Description, bool IsSystem);

public sealed record CreateUserRequest(
    string UserName,
    string FullName,
    string? Email,
    string? Phone,
    string Password,
    Guid RoleId,
    IReadOnlyList<Guid> BranchIds);

public sealed record SetPinRequest(string? Pin);

public sealed record ResetUserPasswordRequest(string NewPassword);

public sealed record SetUserActiveRequest(bool IsActive);

public sealed record LoginHistoryItemDto(Guid Id, string UserName, bool IsSuccess, string? FailureReason, string? IpAddress, DateTime AttemptedAt);

/// <summary>Theme: Light, Dark or Auto (no saved choice). Language: en or ur.</summary>
public sealed record UserPreferencesDto(string Theme, string Language);
