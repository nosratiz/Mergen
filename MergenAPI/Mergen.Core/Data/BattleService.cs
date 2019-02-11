namespace Mergen.Core.Data
{
    public abstract class BattleService
    {
        public Battle Battle { get; set; }

        public BattleService(Battle battle)
        {
            Battle = battle;
        }
    }
}