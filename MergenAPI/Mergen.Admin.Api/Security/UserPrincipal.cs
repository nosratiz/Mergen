using System.Security.Claims;
using System.Security.Principal;
using Mergen.Admin.Api.Security.AuthorizationSystem;

namespace Mergen.Admin.Api.Security
{
    public class AccountPrincipal : ClaimsPrincipal
    {
        public AccountPrincipal(int accountId, string accountEmail, string timezone, IPrincipal principal) :
            base(principal)
        {
            AccountId = accountId;
            AccountEmail = accountEmail;
            Timezone = timezone;
        }

        public Permission[] Permissions { get; set; }
        public int AccountId { get; set; }
        public string AccountEmail { get; set; }
        public string Timezone { get; set; }
    }
}