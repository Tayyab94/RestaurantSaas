namespace RestaurantSaaS.Contracts.Dtos;

public enum LicenseState
{
    NotActivated, Trial, Active, Expiring, Grace, Expired,
    Tampered, ClockError, Mismatch, Revoked, NotRequired,
}

public static class LicenseStateExtensions
{
    public static bool BlocksSelling(this LicenseState state) =>
        state is LicenseState.NotActivated or LicenseState.Expired or LicenseState.Tampered
            or LicenseState.ClockError or LicenseState.Mismatch or LicenseState.Revoked;
}

public sealed record LicenseStatusDto(
    LicenseState State, string LicenseType, string? PlanName, string? LicensedTo, string? KeyHint,
    DateTime? ExpiresAtUtc, int? DaysLeft, int? GraceDaysLeft, int MaxDevices, int MaxBranches,
    bool BlocksSelling, string Message, string Level);

public sealed record ActivateLicenseRequest(string Key, string UserName);
public sealed record StartTrialRequest(string UserName, string Phone, string? Email);

public sealed record ServerActivateRequest(
    string Key, string UserName, string Fingerprint, IReadOnlyList<string> FingerprintParts,
    string? MachineName, string? OsVersion, string? AppVersion, string DeploymentMode);

public sealed record ServerTrialRequest(
    string UserName, string Phone, string? Email, string Fingerprint, IReadOnlyList<string> FingerprintParts,
    string? MachineName, string? OsVersion, string? AppVersion, string DeploymentMode);

public sealed record LicenseIssueDto(string LicenseFile);

/// The app sends its signed license file as proof that it holds the activation it wants to free.
public sealed record ServerDeactivateRequest(string LicenseFile, string? Reason);

public sealed record GenerateLicenseKeysRequest(
    string LicenseType, Guid PlanId, int Quantity, int MaxActivations,
    int MaxDevices, int MaxBranches, string? Notes, decimal? SoldPrice);

public sealed record GeneratedLicenseKeysDto(string BatchNumber, IReadOnlyList<string> Keys);

public sealed record LicenseKeyListItemDto(
    Guid Id, string KeyHint, string LicenseType, string PlanName, string Status,
    int ActivationCount, int MaxActivations, string? LicensedUserName, DateTime? ExpiresAt, DateTime CreatedAt);

public sealed record PlanOptionDto(Guid Id, string Code, string Name);
