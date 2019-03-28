using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Accounts
{
    public class FriendRequestInputModel
    {
        [Required]
        public long FriendAccountId { get; set; }
    }
}