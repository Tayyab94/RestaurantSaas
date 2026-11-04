using System.Security.Cryptography;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Infrastructure.Services;

public sealed class OpaqueTokenService(IClock clock) : ITokenService
{
    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(CurrentUserDto user) =>
        (Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), clock.UtcNow.AddMinutes(15));
}
