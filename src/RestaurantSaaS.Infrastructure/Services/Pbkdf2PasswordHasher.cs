using System.Security.Cryptography;
using RestaurantSaaS.Application.Abstractions;

namespace RestaurantSaaS.Infrastructure.Services;

/// <summary>
/// Password hashing with PBKDF2-HMAC-SHA256, a random salt per password and 600,000 iterations (OWASP guidance).
/// Stored as "v1.iterations.salt.hash" so the cost can be raised later without breaking old hashes.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 600_000;

    private string? _dummyHash;

    public string DummyHash => _dummyHash ??= Hash(Convert.ToBase64String(RandomNumberGenerator.GetBytes(16)));

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.');

        if (parts.Length != 4 || parts[0] != "v1" || !int.TryParse(parts[1], out var iterations) || iterations < 1)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
