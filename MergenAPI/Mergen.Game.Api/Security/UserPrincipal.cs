using System.Security.Claims;
using System.Security.Principal;
using Mergen.Game.Api.Security.AuthorizationSystem;

namespace Mergen.Game.Api.Security
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