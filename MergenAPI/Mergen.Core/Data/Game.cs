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

        public long PlayerId { get; set; }
        public Account Player { get; set; }

        public long? SelectedCategoryId { get; set; }
        public Category SelectedCategory { get; set; }

        public ICollection<GameCategory> GameCategories { get; set; }
        public ICollection<GameQuestion> GameQuestions { get; set; }

        public GameState GameState { get; set; }
    }
}