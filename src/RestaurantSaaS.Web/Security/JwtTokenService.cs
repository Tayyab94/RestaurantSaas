using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Web.Security;

/// <summary>Creates the short-lived signed access token. Permissions travel inside it, so API calls need no extra lookup.</summary>
internal sealed class JwtTokenService(IOptions<JwtOptions> options, IClock clock) : ITokenService
{
    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(CurrentUserDto user)
    {
        var settings = options.Value;
        var expires = clock.UtcNow.AddMinutes(settings.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("name", user.UserName),
            new("is_super_admin", user.IsSuperAdmin ? "true" : "false"),
        };

        if (user.TenantId is { } tenantId)
        {
            claims.Add(new Claim("tenant_id", tenantId.ToString()));
        }

        claims.AddRange(user.Roles.Select(role => new Claim("role", role)));
        claims.AddRange(user.Permissions.Select(permission => new Claim("perm", permission)));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
                SecurityAlgorithms.HmacSha256),
        };

        return (new JsonWebTokenHandler().CreateToken(descriptor), expires);
    }
}
