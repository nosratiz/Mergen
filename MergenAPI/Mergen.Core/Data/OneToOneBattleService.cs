namespace Mergen.Core.Data
{
    public class OneToOneBattleService : BattleService
    {
        private readonly DataContext _dataContext;
        private readonly OneToOneBattle _battle;

        public OneToOneBattleService(DataContext dataContext, OneToOneBattle battle) : base(battle)
        {
            _dataContext = dataContext;
            _battle = battle;
        }
    }
}