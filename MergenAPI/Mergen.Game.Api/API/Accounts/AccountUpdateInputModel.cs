using System;
using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Accounts
{
    public class AccountUpdateInputModel
    {
        public string Nickname { get; set; }
        public string GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }
        public bool ReceiveNotifications { get; set; }
        public bool SearchableByEmailAddressOrUsername { get; set; }
        public bool FriendsOnlyBattleInvitations { get; set; }
    }
}