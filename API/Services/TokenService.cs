using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        string tokenKey = configuration.GetValue<string>("TokenKey") ?? throw new Exception("Cannot access tokenKey from appsettings");
        if (tokenKey.Length < 64)
        {
            throw new Exception("TokenKey must be at least 64 characters long");
        }

        SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(tokenKey));

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Username),
        ];

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
