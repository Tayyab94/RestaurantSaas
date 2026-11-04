namespace RestaurantSaaS.Web.Security;

/// <summary>Bound from the "Jwt" section. The signing key is a secret: set it with an environment variable in production.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "RestaurantSaaS";

    public string Audience { get; set; } = "RestaurantSaaS.Clients";

    public string SigningKey { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; } = 15;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SigningKey) || SigningKey.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey must be set and at least 32 characters long. In production set it with the environment variable Jwt__SigningKey.");
        }
    }
}
