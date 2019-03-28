using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mergen.Api.Core.Security.AuthorizationSystem
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (!(context.User is AccountPrincipal accountPrincipal))
                return _next(context);

            var permissions = new List<Permission>();

            accountPrincipal.Permissions = permissions.ToArray();

            return _next(context);
        }
    }
}