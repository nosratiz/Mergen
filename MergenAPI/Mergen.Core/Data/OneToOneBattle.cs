using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mergen.Core.Entities;

namespace Mergen.Core.Data
{
    public class OneToOneBattle : Battle
    {
        public OneToOneBattle()
        {
            Games = new Collection<Game>();
        }

        public long Player1Id { get; set; }
        public Account Player1 { get; set; }

        public long? Player2Id { get; set; }
        public Account Player2 { get; set; }
        public int Round { get; set; }
        public long LastGameId { get; set; }
        public Game LastGame { get; set; }

        public long? WinnerPlayerId { get; set; }
        public int Player1CorrectAnswersCount { get; set; }
        public int Player2CorrectAnswersCount { get; set; }

        public ICollection<Game> Games { get; set; }

        public BattleState BattleState { get; set; }
    }
}