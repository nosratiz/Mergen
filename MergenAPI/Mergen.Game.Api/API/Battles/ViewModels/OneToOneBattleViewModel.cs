using Mergen.Core.EntityIds;

namespace Mergen.Game.Api.API.Battles.ViewModels
{
    public class OneToOneBattleViewModel : BattleViewModel
    {
        public long? Player1Id { get; set; }
        public PlayerMiniProfileViewModel Player1MiniProfile { get; set; }
        public long? Player2Id { get; set; }
        public PlayerMiniProfileViewModel Player2MiniProfile { get; set; }
        public int Round { get; set; }
        public long? LastGameId { get; set; }
        public long? WinnerPlayerId { get; set; }
        public int Player1CorrectAnswersCount { get; set; }
        public int Player2CorrectAnswersCount { get; set; }
        public int CurrentTurnPlayerId { get; set; }

        public BattleStateIds BattleStateId { get; set; }
    }
}