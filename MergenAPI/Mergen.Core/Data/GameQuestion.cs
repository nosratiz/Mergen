using Mergen.Core.Entities;

namespace Mergen.Core.Data
{
    public class GameQuestion
    {
        public long GameId { get; set; }
        public Game Game { get; set; }

        public long QuestionId { get; set; }
        public Question Question { get; set; }

        public int? Player1SelectedAnswer { get; set; }
        public int? Player2SelectedAnswer { get; set; }

        public int Score { get; set; }
    }
}