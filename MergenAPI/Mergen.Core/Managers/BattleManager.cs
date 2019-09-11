using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Mergen.Core.Managers
{
    public class BattleManager :EntityManagerBase<OneToOneBattle>
    {
        public BattleManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }
    }
}