using Mergen.Api.Core.QueryProcessing;
using Mergen.Core.Entities;
using Mergen.Core.QueryProcessing;

namespace Mergen.Game.Api.API.Accounts.InputModels
{
    public class FriendRequestFilterInputModel
    {
        [ValidOperators(Op.Equals)]
        public long FromAccountId { get; set; }
        public long ToAccountId { get; set; }
        public FriendRequestStatus StatusId { get; set; }
    }
}