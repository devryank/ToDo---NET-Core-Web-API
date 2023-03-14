using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDo.Helper
{
    public static class JwtHelper
    {
        public static IEnumerable<Claim> decodeJwt(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtDecoded = tokenHandler.ReadJwtToken(token);
            var claims = jwtDecoded.Claims;
            return claims;
        }
        public static string decodeJwt(string token, string dataReturn)
        {
            var claims = decodeJwt(token);

            var data = claims.FirstOrDefault(u => u.Type == "Id")?.Value;

            return data;
        }
    }
}
