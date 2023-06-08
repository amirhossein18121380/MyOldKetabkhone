using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MyApi.Helpers
{
    public static class JwtHelper
    {
        private static readonly string _audience = "localhost";
        private static readonly string _issuer = "localhost";

        public static string GenerateToken(long userId, string secret, string[] roles = null)
        {
            //var _secret = _config["Jwt:Key"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userId.ToString()),
                    new Claim("UserId",userId.ToString()),
                    new Claim("RefreshTime", DateTime.Now.AddMinutes(15).Ticks.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                Audience = _audience,
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string jwtToken, string secret)
        {
            IdentityModelEventSource.ShowPII = true;

            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidAudience = _audience.ToLower(),
                ValidIssuer = _issuer.ToLower(),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out _);

            return principal;
        }
    }
}
