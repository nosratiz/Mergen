using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Mergen.Api.Core.Security.AuthenticationSystem
{
    public static class AuthenticationConfigExtensions
    {
        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var tokenConfig = new JwtTokenOptions();
            configuration.GetSection("JwtTokenOptions").Bind(tokenConfig);

            var tokenParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                SaveSigninToken = false,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidIssuer = tokenConfig.Issuer,
                ClockSkew = tokenConfig.Expiretion,
                ValidAudience = tokenConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.SecretKey))
            };

            var authBuilder = services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = false;
                    options.RequireHttpsMetadata = false;
                    options.ClaimsIssuer = tokenConfig.Issuer;
                    options.TokenValidationParameters = tokenParameters;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            if (ctx.HttpContext.Request.Query.TryGetValue("authorization", out var authValues) &&
                                authValues.Any())
                            {
                                ctx.Token = authValues[0];

                                const string bearerPrefix = "Bearer ";
                                if (ctx.Token.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                                    ctx.Token = ctx.Token.Substring(bearerPrefix.Length);
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            return authBuilder;
        }
    }
}