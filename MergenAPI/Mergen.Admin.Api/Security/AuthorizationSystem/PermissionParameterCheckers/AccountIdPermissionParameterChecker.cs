namespace Mergen.Admin.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class AccountIdPermissionParameterChecker : PermissionParameterChecker<int>
    {
        public AccountIdPermissionParameterChecker(int value) : base("AccountId", value)
        {
        }
    }
}