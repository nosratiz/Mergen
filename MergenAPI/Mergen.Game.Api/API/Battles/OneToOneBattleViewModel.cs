﻿using Mergen.Core.Data;

namespace Mergen.Game.Api.API.Battles
{
    public class OneToOneBattleViewModel : BattleViewModel
    {
        public long? Player1Id { get; set; }
        public PlayerMiniProfileViewModel Player1MiniProfile { get; set; }
        public long? Player2Id { get; set; }
        public PlayerMiniProfileViewModel Player2MiniProfile { get; set; }
        public int Round { get; set; }
        public long LastGameId { get; set; }
        public long? WinnerPlayerId { get; set; }

        public BattleState BattleState { get; set; }
    }
}