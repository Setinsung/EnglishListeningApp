using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Commons.JWT;

public class TokenService : ITokenService
{
    public string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
    {
        TimeSpan ExpiryDuration = TimeSpan.FromSeconds(options.ExpireSeconds);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.Now.Add(ExpiryDuration),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
