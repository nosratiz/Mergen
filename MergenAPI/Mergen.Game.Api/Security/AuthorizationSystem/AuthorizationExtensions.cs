using Microsoft.AspNetCore.Builder;

namespace Mergen.Game.Api.Security.AuthorizationSystem
{
    public static class AuthorizationExtensions
    {
        public static void UseAuthorization(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}