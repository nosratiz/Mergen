namespace Mergen.Game.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class IntegerPermissionParameterChecker : PermissionParameterChecker<long>
    {
        public IntegerPermissionParameterChecker(string key, long value) : base(key, value)
        {
        }
    }
}