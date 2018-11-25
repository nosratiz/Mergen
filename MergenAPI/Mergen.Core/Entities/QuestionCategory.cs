using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class QuestionCategory : Entity
    {
        public long QuestionId { get; set; }
        public long CategoryId { get; set; }
    }
}