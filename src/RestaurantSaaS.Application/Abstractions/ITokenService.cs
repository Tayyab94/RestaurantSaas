using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Application.Abstractions;

public interface ITokenService
{
    /// <summary>Creates a short-lived signed access token (JWT) for the user.</summary>
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(CurrentUserDto user);
}
