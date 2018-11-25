using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mergen.Admin.Api.Security.AuthenticationSystem;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Helpers;
using Mergen.Core.Managers;
using Microsoft.AspNetCore.Http;

namespace Mergen.Admin.Api.Security
{
    public class PrincipalWrapperMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly SessionManager _sessionManager;
        private readonly AccountManager _accountManager;

        public PrincipalWrapperMiddleware(RequestDelegate next, JwtTokenGenerator jwtTokenGenerator,
            SessionManager sessionManager, AccountManager accountManager)
        {
            _next = next;
            _jwtTokenGenerator = jwtTokenGenerator;
            _sessionManager = sessionManager;
            _accountManager = accountManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var accountId = int.Parse(context.User.Claims.First(q => q.Type == JwtRegisteredClaimNames.Jti).Value);
                var accountEmail = context.User.Claims
                    .FirstOrDefault(c =>
                        c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                    .NormalizeEmail();

                var timezone = context.User.Claims
                    .FirstOrDefault(c => string.Equals(c.Type, "Timezone", StringComparison.OrdinalIgnoreCase))?.Value;
                if (timezone == null)
                {
                    var profile = await _accountManager.GetAsync(accountId);
                    timezone = profile.Timezone;

                    var token = _jwtTokenGenerator.GenerateToken(TimeSpan.FromDays(365),
                        new Claim(JwtRegisteredClaimNames.Jti, accountId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, accountEmail),
                        new Claim("Timezone", timezone));

                    var session = new Session
                    {
                        AccessToken = token,
                        AccountId = accountId,
                        CreationDateTime = DateTime.UtcNow,
                        StateId = SessionStateIds.Created
                    };

                    session = await _sessionManager.SaveAsync(session);
                    context.Response.Headers.Add("Set-Authorization", session.AccessToken);
                }

                context.User = new AccountPrincipal(accountId, accountEmail, timezone, context.User);
            }

            await _next(context);
        }
    }
}