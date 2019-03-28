namespace Mergen.Api.Core.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class BooleanPermissionParameterChecker : PermissionParameterChecker<bool>
    {
        public BooleanPermissionParameterChecker(string key, bool value) : base(key, value)
        {
        }
    }
}