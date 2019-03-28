using System.Linq;
using System.Security.Principal;

namespace Mergen.Api.Core.Security.AuthorizationSystem
{
    public static class PrincipalExtensions
    {
        public static bool HasPermission(this IPrincipal principal, string permission,
            params IPermissionParameterChecker[] checkers)
        {
            if (!(principal is AccountPrincipal accountPrincipal))
                return false;

            foreach (var principalPermission in accountPrincipal.Permissions)
            {
                if (principalPermission.Key != permission)
                    continue;

                foreach (var principalPermissionParameter in principalPermission.Parameters)
                {
                    var checker = checkers.FirstOrDefault(q => q.Key == principalPermissionParameter.Key);

                    if (checker == null || !checker.CheckParameter(principalPermissionParameter.Values))
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}