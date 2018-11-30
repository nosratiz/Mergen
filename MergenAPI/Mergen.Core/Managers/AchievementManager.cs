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
    public class AchievementManager : EntityManagerBase<Achievement>
    {
        public AchievementManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public Task<Achievement> GetAsync(long accountId, long achievementTypeId, CancellationToken cancellationToken)
        {
            return FirstOrDefaultAsync(q => q.AccountId == accountId && q.AchievementTypeId == achievementTypeId, cancellationToken);
        }
    }
}