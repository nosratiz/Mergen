using Mergen.Core.QueryProcessing;

namespace Mergen.Admin.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class AccountIdQueryPermissionParameterChecker : QueryPermissionChecker
    {
        public AccountIdQueryPermissionParameterChecker(QueryInputModel value) : base("AccountId", value)
        {
        }

        public AccountIdQueryPermissionParameterChecker(QueryInputModel value, string queryFieldName) : base(
            "AccountId", value, queryFieldName)
        {
        }
    }
}