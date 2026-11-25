namespace RestaurantSaaS.Application.Features.Settings;

public enum BillRoundingMode
{
    None,
    Nearest,
    Up,
    Down,
}

/// <summary>A tax that applies to the bill. A fixed tax is a flat amount per bill and is never included in prices.</summary>
public sealed record BillTaxRule(string Name, decimal Rate, bool IsFixed, bool IsInclusive);

public sealed record BillRounding(BillRoundingMode Mode, int Step);

public sealed record BillTaxLine(string Name, decimal Rate, decimal Amount, bool IsInclusive);

public sealed record BillResult(decimal Subtotal, decimal ServiceCharge, IReadOnlyList<BillTaxLine> Taxes, decimal Rounding, decimal Total);

/// <summary>
/// The one place that works out service charge, tax and rounding of a bill (pure, no database).
/// M05 uses it for the test bill; M11 Billing must use the same class.
/// Rules: the service charge is a percent of the items total. Tax is worked out on the items total only (the service
/// charge is not taxed). An exclusive tax is added to the total; an inclusive tax is already inside the prices and is only
/// shown. Rounding is applied last, to the total.
/// </summary>
public static class BillCalculator
{
    public static BillResult Calculate(
        decimal subtotal,
        decimal serviceChargePercent,
        IReadOnlyList<BillTaxRule> taxes,
        BillRounding rounding,
        int decimalPlaces)
    {
        var service = serviceChargePercent > 0
            ? Math.Round(subtotal * serviceChargePercent / 100m, decimalPlaces, MidpointRounding.AwayFromZero)
            : 0m;

        var lines = new List<BillTaxLine>();
        var added = 0m;

        foreach (var tax in taxes)
        {
            var inclusive = tax.IsInclusive && !tax.IsFixed;

            var amount = tax.IsFixed
                ? tax.Rate
                : inclusive
                    ? subtotal * tax.Rate / (100m + tax.Rate)
                    : subtotal * tax.Rate / 100m;

            amount = Math.Round(amount, decimalPlaces, MidpointRounding.AwayFromZero);
            lines.Add(new BillTaxLine(tax.Name, tax.Rate, amount, inclusive));

            if (!inclusive)
            {
                added += amount;
            }
        }

        var before = subtotal + service + added;
        var step = Math.Max(1, rounding.Step);

        var total = rounding.Mode switch
        {
            BillRoundingMode.Nearest => Math.Round(before / step, 0, MidpointRounding.AwayFromZero) * step,
            BillRoundingMode.Up => Math.Ceiling(before / step) * step,
            BillRoundingMode.Down => Math.Floor(before / step) * step,
            _ => before,
        };

        return new BillResult(subtotal, service, lines, total - before, total);
    }
}
