using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Mergen.Game.Api.Security.AuthenticationSystem
{
    public class JwtTokenGenerator
    {
        private readonly JwtTokenOptions _options;

        public JwtTokenGenerator(IOptions<JwtTokenOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(TimeSpan expiresAfter, params Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expiresAfter),
                signingCredentials: creds);

            var tokenResult = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return tokenResult;
        }

        public SecurityToken ReadToken(string securityToken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(securityToken);
        }
    }
}