namespace RestaurantSaaS.Contracts.Dtos;

public sealed record PermissionDto(string Key, string Module, string Description);

/// <summary>What a role may approve with a manager PIN. The Owner role is unlimited.</summary>
public sealed record ApprovalLimitDto(decimal MaxDiscountPercent, decimal MaxDiscountAmount, bool CanApproveVoid, bool CanApproveReturn);

public sealed record RoleDetailDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystem,
    int UserCount,
    IReadOnlyList<string> Permissions,
    ApprovalLimitDto Limits);

/// <summary>Id is null for a new role.</summary>
public sealed record SaveRoleRequest(
    Guid? Id,
    string Name,
    string? Description,
    IReadOnlyList<string> Permissions,
    ApprovalLimitDto Limits);

public sealed record BranchDto(Guid Id, string Code, string Name, bool IsMain);

public sealed record SetUserRoleRequest(Guid RoleId);

public sealed record SetUserBranchesRequest(IReadOnlyList<Guid> BranchIds);

/// <summary>A cashier or waiter needs a manager's OK (discount above the limit, void, return...): the manager types the username and PIN.</summary>
/// <param name="ApprovalType">Void, Discount, Return, CashOut or PriceOverride.</param>
public sealed record ApprovalRequest(
    string ApprovalType,
    Guid BranchId,
    string ApproverUserName,
    string Pin,
    string ReferenceType,
    Guid ReferenceId,
    decimal? Amount,
    decimal? Percent,
    string? Reason);

public sealed record ApprovalResultDto(Guid ApprovalId, string ApproverName, string ApprovalType);

public sealed record AuditLogItemDto(
    Guid Id,
    DateTime OccurredAt,
    string? UserName,
    string Action,
    string EntityName,
    Guid EntityId,
    string? Details,
    string? IpAddress);
