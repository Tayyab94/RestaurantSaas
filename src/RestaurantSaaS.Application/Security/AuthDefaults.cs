using System.Security.Cryptography;

namespace RestaurantSaaS.Application.Security;

public static class AuthDefaults
{
    public const int MaxFailedAttempts = 5;
    public const int LockoutMinutes = 15;
    public const int RefreshTokenDays = 14;
    public const int TrialDays = 14;
}

internal static class RefreshTokenGenerator
{
    public static (string Token, string Hash) Generate()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
        return (token, Hash(token));
    }

    public static string Hash(string token) =>
        Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
}
