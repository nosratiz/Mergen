using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountFriend : Entity
    {
        public long AccountId { get; set; }
        public long FriendAccountId { get; set; }
    }
}