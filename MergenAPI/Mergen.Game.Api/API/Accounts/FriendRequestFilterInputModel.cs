using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Accounts
{
    public class FriendRequestFilterInputModel
    {
        public long FromAccountId { get; set; }
        public long ToAccountId { get; set; }
        public FriendRequestStatus StatusId { get; set; }
    }
}