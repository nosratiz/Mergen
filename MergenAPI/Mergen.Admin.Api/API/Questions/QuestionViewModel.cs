using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Api.Core.ViewModels;
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
        public long Answer1ChooseHistory { get; set; }
        public long Answer2ChooseHistory { get; set; }
        public long Answer3ChooseHistory { get; set; }
        public long Answer4ChooseHistory { get; set; }
        public int CorrectAnswerNumber { get; set; }
        public IEnumerable<string> CategoryIds { get; set; }

        public static QuestionViewModel Map(Question question)
        {
            return new QuestionViewModel
            {
                Id = question.Id.ToString(),
                IsArchived = question.IsArchived,
                Body = question.Body,
                Difficulty = question.Difficulty,
                Answer1 = question.Answer1,
                Answer2 = question.Answer2,
                Answer3 = question.Answer3,
                Answer4 = question.Answer4,
                Answer1ChooseHistory = question.Answer1ChooseHistory,
                Answer2ChooseHistory = question.Answer2ChooseHistory,
                Answer3ChooseHistory = question.Answer3ChooseHistory,
                Answer4ChooseHistory = question.Answer4ChooseHistory,
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