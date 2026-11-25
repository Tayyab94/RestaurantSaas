using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Application.Features.Settings;

/// <summary>The keys and groups of the Setting table used by M05. A branch value has a BranchId; a business value has none.</summary>
internal static class SettingKeys
{
    public const string BusinessGroup = "Business";
    public const string BranchGroup = "Branch";
    public const string BillingGroup = "Billing";

    public const string BusinessAddress = "business.address";
    public const string BusinessTaxNumber = "business.taxNumber";
    public const string BusinessLogo = "business.logo";

    public const string BranchOpeningHours = "branch.openingHours";

    public const string ServiceChargeEnabled = "billing.serviceCharge.enabled";
    public const string ServiceChargePercent = "billing.serviceCharge.percent";
    public const string RoundingMode = "billing.rounding.mode";
    public const string RoundingStep = "billing.rounding.step";
}

/// <summary>What the Owner chose for service charge and rounding.</summary>
internal sealed record BillingSettings(bool ServiceChargeEnabled, decimal ServiceChargePercent, BillRoundingMode RoundingMode, int RoundingStep);

/// <summary>
/// Reads and writes Setting rows. It only adds or changes rows; the calling handler saves them
/// together with its own change.
/// </summary>
internal sealed class SettingStore(IApplicationDbContext db)
{
    public async Task<Dictionary<string, string?>> GetAsync(Guid? branchId, string[] keys, CancellationToken ct)
    {
        var rows = await db.Set<Setting>().AsNoTracking()
            .Where(s => s.BranchId == branchId && keys.Contains(s.Key))
            .Select(s => new { s.Key, s.Value })
            .ToListAsync(ct);

        return rows
            .GroupBy(r => r.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);
    }

    /// <summary>Sets a value. An empty value removes the row.</summary>
    public async Task SetAsync(Guid tenantId, Guid? branchId, string group, string key, string? value, string valueType, CancellationToken ct)
    {
        var row = await db.Set<Setting>().FirstOrDefaultAsync(s => s.BranchId == branchId && s.Key == key, ct);

        if (string.IsNullOrEmpty(value))
        {
            if (row is not null)
            {
                db.Set<Setting>().Remove(row);
            }

            return;
        }

        if (row is null)
        {
            db.Set<Setting>().Add(new Setting
            {
                TenantId = tenantId,
                BranchId = branchId,
                Group = group,
                Key = key,
                Value = value,
                ValueType = valueType,
                IsEncrypted = false,
            });
        }
        else
        {
            row.Value = value;
        }
    }

    public async Task<BillingSettings> GetBillingAsync(CancellationToken ct)
    {
        var values = await GetAsync(
            null,
            [SettingKeys.ServiceChargeEnabled, SettingKeys.ServiceChargePercent, SettingKeys.RoundingMode, SettingKeys.RoundingStep],
            ct);

        var enabled = Read(values, SettingKeys.ServiceChargeEnabled) == "true";

        var percent = decimal.TryParse(Read(values, SettingKeys.ServiceChargePercent), NumberStyles.Number, CultureInfo.InvariantCulture, out var p)
            ? p
            : 0m;

        var mode = Enum.TryParse<BillRoundingMode>(Read(values, SettingKeys.RoundingMode), out var m) ? m : BillRoundingMode.None;
        var step = int.TryParse(Read(values, SettingKeys.RoundingStep), NumberStyles.Integer, CultureInfo.InvariantCulture, out var s) && s > 0 ? s : 1;

        return new BillingSettings(enabled, percent, mode, step);
    }

    private static string? Read(Dictionary<string, string?> values, string key) =>
        values.TryGetValue(key, out var value) ? value : null;
}
