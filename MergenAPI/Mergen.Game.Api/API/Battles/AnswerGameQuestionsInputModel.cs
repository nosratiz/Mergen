using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Battles
{
    public class AnswerGameQuestionsInputModel
    {
        [MinLength(1)]
        [Required]
        public IEnumerable<QuestionAnswerInputModel> Answers { get; set; }

        public class QuestionAnswerInputModel
        {
            public long QuestionId { get; set; }
            public int SelectedAnswer { get; set; }
            public bool UsedRemoveTwoAnswersHelper { get; set; }
            public bool UsedAnswersHistoryHelper { get; set; }
            public bool UsedAskMergenHelper { get; set; }
        }
    }
}