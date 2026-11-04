namespace RestaurantSaaS.Contracts.Dtos;

/// <summary>What the sign-in and sign-up screens need to know about this installation.</summary>
/// <param name="DeploymentMode">Cloud, OnPremise or Desktop.</param>
/// <param name="HasBusiness">A business already exists.</param>
/// <param name="AllowsRegistration">Cloud: always. Desktop and on-premise: only until the one business is set up.</param>
/// <param name="SingleBusinessCode">Desktop and on-premise: the code of the one business, so sign-in can fill it in.</param>
public sealed record SetupStatusDto(string DeploymentMode, bool HasBusiness, bool AllowsRegistration, string? SingleBusinessCode);

public sealed record OnboardingStepDto(string Key, string Title, string Description, string? Route, bool IsDone, bool IsAutomatic);

public sealed record OnboardingChecklistDto(IReadOnlyList<OnboardingStepDto> Steps, int DoneCount, int TotalCount);

public sealed record SetOnboardingStepRequest(bool IsDone);
