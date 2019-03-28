namespace Mergen.Api.Core.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class StringPermissionParameterChecker : PermissionParameterChecker<string>
    {
        public StringPermissionParameterChecker(string key, string value) : base(key, value)
        {
        }
    }
}