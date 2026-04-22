using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using x99AssessmentByTva.Application.Common.Interfaces;

namespace x99AssessmentByTva.Infrastructure.Identity;

public sealed class TokenService(
    IConfiguration configuration) : ITokenService
{
    public (string Token, DateTime ExpiresAt) CreateToken(
        string userId,
        string email,
        string? displayName,
        IEnumerable<string> roles)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "120"));

        return
        (
            Token: new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken
            (
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: BuildClaims
                (
                    userId,
                    email,
                    displayName,
                    roles
                ),
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials
                (
                    key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]!)),
                    algorithm: SecurityAlgorithms.HmacSha256
                )
            )),
            ExpiresAt: expiresAt
        );
    }

    private static List<Claim> BuildClaims(
        string userId, 
        string email, 
        string? displayName, 
        IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            //INFO: JWT standard claims
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Email, email),

            //INFO: ASP.NET Core identity claims
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, displayName ?? string.Empty),
            new(ClaimTypes.Email,email),
        };

        claims.AddRange
        (
            roles.Select(role => new Claim
            (
                ClaimTypes.Role,
                role
            )
        ));

        return claims;
    }
}
