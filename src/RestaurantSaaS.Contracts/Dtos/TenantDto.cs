namespace RestaurantSaaS.Contracts.Dtos;

public sealed record TenantDto(
    Guid Id,
    string Code,
    string Name,
    string BusinessType,
    string Status,
    string DeploymentMode,
    DateTime CreatedAt);
