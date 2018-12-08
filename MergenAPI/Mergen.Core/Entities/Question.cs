using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Question : Entity
    {
        public string Body { get; set; }
        public int Difficulty { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswerNumber { get; set; }
        public string CategoryIdsCache { get; set; } 
    }
}