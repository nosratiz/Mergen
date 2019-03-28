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
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Managers
{
    public class AccountFriendManager : EntityManagerBase<AccountFriend>
    {
        public AccountFriendManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }


        public async Task<IEnumerable<(Account account, AccountStatsSummary stats)>> GetFriendsAsync(long accountId, CancellationToken cancellationToken)
        {
            using (var dbc = CreateDbContext())
            {
                var result = await dbc.AccountFriends.Where(q => q.AccountId == accountId)
                    .Join(dbc.Accounts, accountFriend => accountFriend.FriendAccountId, account => account.Id, (accountFriend, account) => account)
                    .Join(dbc.AccountStatsSummaries, account => account.Id, stats => stats.AccountId, (account, stats) => new { account, stats })
                    .ToListAsync(cancellationToken);

                return result.Select(q => (q.account, q.stats));
            }
        }

        public async Task<bool> IsFriendAsync(long accountId, long friendAccountId, CancellationToken cancellationToken)
        {
            using (var dbc = CreateDbContext())
            {
                return await dbc.AccountFriends.AnyAsync(q=>q.AccountId == accountId && q.FriendAccountId == friendAccountId, cancellationToken);
            }
        }
    }
}