using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Mergen.Core.Helpers;
using Microsoft.AspNetCore.Http;

namespace Mergen.Api.Core.Security
{
    public class PrincipalWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public PrincipalWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
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

                context.User = new AccountPrincipal(accountId, accountEmail, timezone, context.User);
            }

            await _next(context);
        }
    }
}