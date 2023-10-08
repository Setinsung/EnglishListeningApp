using System.Security.Claims;

namespace Commons.JWT;

public interface ITokenService
{
    string BuildToken(IEnumerable<Claim> claims, JWTOptions options);
}
