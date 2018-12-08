using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.Managers;

namespace Mergen.Admin.Api.API.Questions
{
    public class QuestionViewModel : EntityViewModel
    {
        private static char[] _categoryIdsCacheSeparator = { ',' };

        public string Body { get; set; }
        public int Difficulty { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswerNumber { get; set; }
        public IEnumerable<string> CategoryIds { get; set; }

        public static QuestionViewModel Map(Question question)
        {
            return new QuestionViewModel
            {
                Id = question.Id.ToString(),
                Body = question.Body,
                Difficulty = question.Difficulty,
                Answer1 = question.Answer1,
                Answer2 = question.Answer2,
                Answer3 = question.Answer3,
                Answer4 = question.Answer4,
                CorrectAnswerNumber = question.CorrectAnswerNumber,
                CategoryIds = question.CategoryIdsCache.Split(_categoryIdsCacheSeparator, StringSplitOptions.RemoveEmptyEntries)
            };
        }

        public static IEnumerable<QuestionViewModel> MapAll(IEnumerable<Question> questions)
        {
            return questions.Select(Map);
        }
    }
}