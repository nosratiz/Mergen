namespace Mergen.Game.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class BooleanPermissionParameterChecker : PermissionParameterChecker<bool>
    {
        public BooleanPermissionParameterChecker(string key, bool value) : base(key, value)
        {
        }
    }
}