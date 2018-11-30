using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.AccountInvitations
{
    public class AccountInvitationViewModel
    {
        public string AccountId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime InvitationDateTime { get; set; }
        public string StatusId { get; set; }
        public string RegisteredAccountId { get; set; }

        public static AccountInvitationViewModel Map(AccountInvitation accountInvitation)
        {
            return AutoMapper.Mapper.Map<AccountInvitationViewModel>(accountInvitation);
        }

        public static IEnumerable<AccountInvitationViewModel> MapAll(IEnumerable<AccountInvitation> accountInvitations)
        {
            return accountInvitations.Select(Map);
        }
    }
}