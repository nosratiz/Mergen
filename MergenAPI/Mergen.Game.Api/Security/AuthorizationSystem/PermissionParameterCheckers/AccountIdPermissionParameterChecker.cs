namespace Mergen.Game.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class AccountIdPermissionParameterChecker : PermissionParameterChecker<int>
    {
        public AccountIdPermissionParameterChecker(int value) : base("CreatorAccountId", value)
        {
        }
    }
}