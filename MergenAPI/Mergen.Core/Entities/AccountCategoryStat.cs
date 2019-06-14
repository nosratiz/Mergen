using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountCategoryStat : Entity
    {
        public long AccountId { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; }
        public long TotalQuestionsCount { get; set; }
        public long CorrectAnswersCount { get; set; }
    }
}