namespace Mergen.Core.Entities
{
    public class GameCategory
    {
        public long GameId { get; set; }
        public Game Game { get; set; }

        public long CategoryId { get; set; }
        public Category Category { get; set; }
    }
}