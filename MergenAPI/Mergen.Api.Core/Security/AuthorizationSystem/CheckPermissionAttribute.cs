using System;
using Mergen.Api.Core.CustomResult;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mergen.Api.Core.Security.AuthorizationSystem
{
    public class CheckPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permissionKey;

        public CheckPermissionAttribute(string permissionKey)
        {
            _permissionKey = permissionKey;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var accountprincipal = context.HttpContext.User as AccountPrincipal;
            if (accountprincipal == null)
            {
                context.Result = new ForbiddenResult();
            }

            accountprincipal.HasPermission(_permissionKey);
        }
    }
}