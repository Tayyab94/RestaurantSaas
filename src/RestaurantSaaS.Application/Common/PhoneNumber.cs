namespace RestaurantSaaS.Application.Common;

/// <summary>
/// Turns the many ways of writing a phone number into one international form (+923001234567),
/// so the same number cannot be registered twice.
/// </summary>
public static class PhoneNumber
{
    public const string DefaultCountryCode = "92";

    public static bool TryNormalize(string? raw, out string normalized, string countryCode = DefaultCountryCode)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var text = raw.Trim();
        var hasPlus = text.StartsWith('+');
        var digits = new string(text.Where(char.IsAsciiDigit).ToArray());

        if (digits.Length == 0)
        {
            return false;
        }

        if (!hasPlus)
        {
            if (digits.StartsWith("00", StringComparison.Ordinal))
            {
                digits = digits[2..];
            }
            else if (digits.StartsWith('0'))
            {
                digits = countryCode + digits[1..];
            }
            else if (digits.Length == 10)
            {
                digits = countryCode + digits;
            }
        }

        if (digits.Length is < 8 or > 15)
        {
            return false;
        }

        normalized = "+" + digits;
        return true;
    }
}
