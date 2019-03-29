using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;

namespace Mergen.Core.Managers
{
    public class FriendRequestManager : EntityManagerBase<FriendRequest>
    {
        public FriendRequestManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public async Task<FriendRequest> GetExistingRequest(long accountId, long friendAccountId, CancellationToken cancellationToken)
        {
            return await FirstOrDefaultAsync(q => q.FromAccountId == accountId && q.ToAccountId == friendAccountId && q.StatusId == FriendRequestStatus.Pending, cancellationToken);
        }
    }
}
