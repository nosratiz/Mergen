using Mergen.Core.Entities;

namespace Mergen.Core.Data
{
    public class GameQuestion
    {
        public long GameId { get; set; }
        public Game Game { get; set; }

        public long QuestionId { get; set; }
        public Question Question { get; set; }

        public int SelectedAnswer { get; set; }
        public int Score { get; set; }
    }
}