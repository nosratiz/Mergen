using Mergen.Api.Core.QueryProcessing;
using Mergen.Api.Core.Security.AuthorizationSystem;
using Mergen.Core.QueryProcessing;

namespace Mergen.Admin.Api.API.Accounts.InputModels
{
    public class AccountFilterInputModel
    {
        [ValidOperators(PermissionKeys.AccountsGet, Op.Equals, Op.In)]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Phone { get; set; }
    }
}