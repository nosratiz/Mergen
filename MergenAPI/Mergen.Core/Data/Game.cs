using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mergen.Core.Entities;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Data
{
    public class Game : Entity
    {
        public Game()
        {
            GameCategories = new Collection<GameCategory>();
            GameQuestions = new Collection<GameQuestion>();
        }

        public long? CurrentTurnPlayerId { get; set; }
        public Account CurrentTurnPlayer { get; set; }

        public long BattleId { get; set; }
        public Battle Battle { get; set; }

        public long? SelectedCategoryId { get; set; }
        public Category SelectedCategory { get; set; }

        public ICollection<GameCategory> GameCategories { get; set; }
        public ICollection<GameQuestion> GameQuestions { get; set; }

        public GameState GameState { get; set; }
        public int Score { get; set; }
    }
}