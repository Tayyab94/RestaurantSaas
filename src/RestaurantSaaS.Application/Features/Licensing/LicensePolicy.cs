using RestaurantSaaS.Contracts.Dtos;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Application.Features.Licensing;

internal static class LicensePolicy
{
    public const int TrialDays = 14;
    public const int GraceDays = 7;
    public const int ExpiringDays = 14;
    public const int ClockToleranceMinutes = 10;
    public const int CacheMinutes = 5;
    public const int MaxFailedAttempts = 10;
    public const int AttemptWindowMinutes = 15;
    public const int MinMatchingParts = 3;

    public static DateTime? ExpiryFor(LicenseType type, int? months, int? days, DateTime from) => type switch
    {
        LicenseType.Lifetime => null,
        LicenseType.Trial => from.AddDays(days ?? TrialDays),
        _ => from.AddMonths(months ?? (type == LicenseType.Months6 ? 6 : 12)),
    };

    public static string TypeName(LicenseType type) => type switch
    {
        LicenseType.Trial => "Trial",
        LicenseType.Months6 => "6 months",
        LicenseType.Months12 => "12 months",
        LicenseType.Lifetime => "Lifetime",
        _ => type.ToString(),
    };
}

internal sealed record LicenseEvaluation(LicenseState State, int? DaysLeft, int? GraceDaysLeft);

internal static class LicenseEvaluator
{
    public static LicenseEvaluation Evaluate(LicenseType type, DateTime? expiresAtUtc, DateTime nowUtc)
    {
        if (expiresAtUtc is null)
        {
            return new LicenseEvaluation(LicenseState.Active, null, null);
        }

        var expires = expiresAtUtc.Value;

        if (nowUtc <= expires)
        {
            var days = (int)Math.Ceiling((expires - nowUtc).TotalDays);

            if (type == LicenseType.Trial)
            {
                return new LicenseEvaluation(LicenseState.Trial, days, null);
            }

            return new LicenseEvaluation(days <= LicensePolicy.ExpiringDays ? LicenseState.Expiring : LicenseState.Active, days, null);
        }

        if (type != LicenseType.Trial)
        {
            var graceEnds = expires.AddDays(LicensePolicy.GraceDays);

            if (nowUtc <= graceEnds)
            {
                return new LicenseEvaluation(LicenseState.Grace, 0, (int)Math.Ceiling((graceEnds - nowUtc).TotalDays));
            }
        }

        return new LicenseEvaluation(LicenseState.Expired, 0, 0);
    }
}

internal static class LicenseStatuses
{
    public static LicenseStatusDto Build(
        LicenseState state, LicenseType type, string? planName, string? licensedTo, string? keyHint,
        DateTime? expiresAtUtc, int? daysLeft, int? graceDaysLeft, int maxDevices, int maxBranches)
    {
        var (message, level) = state switch
        {
            LicenseState.Trial => ($"Free trial: {Days(daysLeft)} left.", daysLeft <= 3 ? "warning" : "info"),
            LicenseState.Active => (expiresAtUtc is null ? "Lifetime license." : $"License active until {expiresAtUtc:dd MMM yyyy}.", "success"),
            LicenseState.Expiring => ($"Your license ends in {Days(daysLeft)}. Renew to keep selling.", "warning"),
            LicenseState.Grace => ($"Your license has ended. Renew within {Days(graceDaysLeft)} or selling will stop.", "danger"),
            LicenseState.Expired => ("Your license has expired. Selling is blocked until you renew.", "danger"),
            LicenseState.NotActivated => ("This installation is not activated. Enter a license key or start the free trial.", "danger"),
            LicenseState.Tampered => ("The license was changed and is not valid. Please contact support.", "danger"),
            LicenseState.ClockError => ("The PC date and time are earlier than the last time the system was used. Set the correct date and time.", "danger"),
            LicenseState.Mismatch => ("The license belongs to another PC or is older than the one used before. Please activate again or contact support.", "danger"),
            LicenseState.Revoked => ("This license was revoked. Please contact support.", "danger"),
            _ => (string.Empty, "info"),
        };

        return new LicenseStatusDto(
            state, LicensePolicy.TypeName(type), planName, licensedTo, keyHint,
            expiresAtUtc, daysLeft, graceDaysLeft, maxDevices, maxBranches,
            state.BlocksSelling(), message, level);
    }

    public static LicenseStatusDto Simple(LicenseState state) =>
        Build(state, LicenseType.Trial, null, null, null, null, null, null, 0, 0);

    private static string Days(int? days) => days == 1 ? "1 day" : $"{days ?? 0} days";
}
