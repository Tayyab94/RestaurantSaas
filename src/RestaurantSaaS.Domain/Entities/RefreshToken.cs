using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>A long-lived sign-in token. Only its hash is stored. Revoked on sign-out and on password change.</summary>
public sealed class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAt) =>
        new() { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt };

    public bool IsActive(DateTime now) => RevokedAt is null && ExpiresAt > now;

    public void Revoke(DateTime now) => RevokedAt ??= now;
}
