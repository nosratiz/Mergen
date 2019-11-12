using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Mergen.Core.Managers
{
    public class BattleManager : EntityManagerBase<OneToOneBattle>
    {
        public BattleManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public async Task<bool> HasPlayedYesterday(long accountId, CancellationToken cancellationToken)
        {
            using (var dbc = CreateDbContext())
            {
                return await dbc.OneToOneBattles.AnyAsync(x =>
                    x.IsArchived == false && (x.Player2Id == accountId || x.Player1Id == accountId) && x.StartDateTime == DateTime.Today.AddDays(-1),
                    cancellationToken);
            }
        }
    }
}