using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Application.Features.Settings;

/// <summary>The currencies a business can choose. A fixed list in code, so no seed data is needed.</summary>
public static class CurrencyCatalog
{
    public static IReadOnlyList<CurrencyDto> All { get; } =
    [
        new("PKR", "Pakistani rupee", "Rs", 2),
        new("USD", "US dollar", "$", 2),
        new("EUR", "Euro", "EUR", 2),
        new("GBP", "British pound", "GBP", 2),
        new("AED", "UAE dirham", "AED", 2),
        new("SAR", "Saudi riyal", "SAR", 2),
        new("INR", "Indian rupee", "INR", 2),
        new("BDT", "Bangladeshi taka", "BDT", 2),
    ];

    public static CurrencyDto? Find(string? code) =>
        All.FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
}
