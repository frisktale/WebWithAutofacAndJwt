using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WebWithAutofacAndJwt.Infrastructure;
public interface IJwtAuthManager
{
    JwtAuthResult GenerateTokens(Claim[] claims, DateTime now);
}

public class JwtAuthManager : IJwtAuthManager
{
    private readonly IOptionsSnapshot<JwtTokenConfig> _jwtTokenConfig;
    private readonly byte[] _secret;

    public JwtAuthManager(IOptionsSnapshot<JwtTokenConfig> jwtTokenConfig)
    {
        _jwtTokenConfig = jwtTokenConfig;
        _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Value.Secret);
    }


    /// <summary>
    /// 颁发token
    /// </summary>
    /// <param name="claims">断言</param>
    /// <param name="now">当前时间</param>
    /// <returns></returns>
    public JwtAuthResult GenerateTokens(Claim[] claims, DateTime now)
    {
        var jwtToken = new JwtSecurityToken(
            _jwtTokenConfig.Value.Issuer,
            _jwtTokenConfig.Value.Audience,
            claims,
            expires: now.AddMinutes(_jwtTokenConfig.Value.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new JwtAuthResult
        {
            AccessToken = accessToken,
        };
    }
}

public class JwtAuthResult
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

}


