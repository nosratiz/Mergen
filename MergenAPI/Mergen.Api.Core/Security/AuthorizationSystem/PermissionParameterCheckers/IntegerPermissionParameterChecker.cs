namespace Mergen.Api.Core.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class IntegerPermissionParameterChecker : PermissionParameterChecker<long>
    {
        public IntegerPermissionParameterChecker(string key, long value) : base(key, value)
        {
        }
    }
}