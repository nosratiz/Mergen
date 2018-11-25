using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountInvitation : Entity
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime InvitationDateTime { get; set; }
        public int StatusId { get; set; }
        public int RegisteredAccountId { get; set; }
    }
}