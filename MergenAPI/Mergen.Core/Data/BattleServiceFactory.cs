using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mergen.Core.Data
{
    public class BattleServiceFactory
    {
        private readonly DataContext _dataContext;

        public BattleServiceFactory(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<BattleService> CreateOneToOneBattleService(Battle battle, CancellationToken cancellationToken)
        {
            switch (battle.BattleType)
            {
                case BattleType.OneOnOne:
                    return new OneToOneBattleService(_dataContext, (OneToOneBattle)battle);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}