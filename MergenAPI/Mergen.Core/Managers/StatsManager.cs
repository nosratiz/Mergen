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
    public class StatsManager : EntityManagerBase<AccountStatsSummary>
    {
        public StatsManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public async Task<AccountStatsSummary> GetByAccountIdAsync(long accountId, CancellationToken cancellationToken = default)
        {
            return (await GetAsync(q => q.AccountId == accountId, cancellationToken)).FirstOrDefault();
        }

        public async Task<int> GetRank(decimal score)
        {
            using (var dbc = CreateDbContext())
            {
                return await dbc.AccountStatsSummaries.CountAsync(x => x.Score >= score);
            }
        }
    }
}
