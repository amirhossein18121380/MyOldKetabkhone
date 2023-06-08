
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Enum;
using DataModel.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyApi.Helpers;

namespace MyApi.Helpers;

public class JwtTokensData
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string RefreshTokenSerial { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
}

public interface ITokenFactoryService
{
    Task<JwtTokensData> CreateJwtTokensAsync(User user, string accessibleResourceIds = null);
    string GetRefreshTokenSerial(string refreshTokenValue);
}

public class TokenFactoryService : ITokenFactoryService
{
    private readonly ISecurityService _securityService;
    private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
    //private readonly IRolesService _rolesService;

    public TokenFactoryService(
        ISecurityService securityService,
        //IRolesService rolesService,
        IOptionsSnapshot<BearerTokensOptions> configuration)
    {
        _securityService = securityService;

        //_rolesService = rolesService;

        _configuration = configuration;
    }

    public async Task<JwtTokensData> CreateJwtTokensAsync(User user, string accessibleResourceIds = null)
    {
        var (accessToken, claims) = await CreateAccessTokenAsync(user, accessibleResourceIds);
        var (refreshTokenValue, refreshTokenSerial) = CreateRefreshToken();
        return new JwtTokensData
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            RefreshTokenSerial = refreshTokenSerial,
            Claims = claims
        };
    }

    private (string RefreshTokenValue, string RefreshTokenSerial) CreateRefreshToken()
    {
        var refreshTokenSerial = _securityService.CreateCryptographicallySecureGuid().ToString().Replace("-", "");

        var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                // for invalidation
                new Claim(ClaimTypes.SerialNumber, refreshTokenSerial, ClaimValueTypes.String, _configuration.Value.Issuer)
            };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _configuration.Value.Issuer,
            audience: _configuration.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
            signingCredentials: creds);
        var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return (refreshTokenValue, refreshTokenSerial);
    }

    public string GetRefreshTokenSerial(string refreshTokenValue)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return null;
        }

        ClaimsPrincipal decodedRefreshTokenPrincipal = null;
        try
        {
            decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                refreshTokenValue,
                new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key)),
                    ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    },
                out _
            );
        }
        catch (Exception ex)
        {
            // LogHelper.ErrorLog($"Failed to validate refreshTokenValue: `{refreshTokenValue}`.", ex);
        }

        return decodedRefreshTokenPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
    }

    private async Task<(string AccessToken, IEnumerable<Claim> Claims)> CreateAccessTokenAsync(User user, string accessibleResourceIds = null)
    {
        var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, _configuration.Value.Issuer),
                //new Claim("DisplayName", user.DisplayName, ClaimValueTypes.String, _configuration.Value.Issuer),
                // to invalidate the cookie
                new Claim(ClaimTypes.SerialNumber, user.UniqCode, ClaimValueTypes.String, _configuration.Value.Issuer),
                // custom data
                new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer)
            };

        if (!string.IsNullOrEmpty(accessibleResourceIds))
        {
            foreach (var resourceId in accessibleResourceIds.Split(","))
            {
                if (short.TryParse(resourceId, out var resId))
                {
                    var roleName = ((ResourcesEnum)resId).ToString();
                    claims.Add(new Claim(ClaimTypes.Role, roleName, ClaimValueTypes.String, _configuration.Value.Issuer));
                }
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _configuration.Value.Issuer,
            audience: _configuration.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
            signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), claims);
    }
}

