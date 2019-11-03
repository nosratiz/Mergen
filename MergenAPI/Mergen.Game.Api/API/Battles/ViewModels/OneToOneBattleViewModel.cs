using Mergen.Core.EntityIds;
using System.Collections.Generic;

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
        public long? CurrentTurnPlayerId { get; set; }

        public List<GameViewModel> Game { get; set; }

        public BattleStateIds BattleStateId { get; set; }
    }
}