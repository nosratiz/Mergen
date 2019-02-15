using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Battles
{
    public class GameViewModel : EntityViewModel
    {
        public CategoryViewModel SelectedCategory { get; set; }
        public IEnumerable<CategoryViewModel> AvailableCategories { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; }

        public static GameViewModel Map(Core.Data.Game game)
        {
            var vm = Mapper.Map<GameViewModel>(game);
            vm.AvailableCategories = game.GameCategories.Select(q => CategoryViewModel.Map(q.Category));
            vm.Questions = game.GameQuestions.Select(q => QuestionViewModel.Map(q.Question));
            return vm;
        }

        public static IEnumerable<GameViewModel> Map(IEnumerable<Core.Data.Game> games)
        {
            return games.Select(Map);
        }
    }

    public class CategoryViewModel : EntityViewModel
    {
        public string Title { get; set; }

        public static CategoryViewModel Map(Category category)
        {
            return Mapper.Map<CategoryViewModel>(category);
        }

        public static IEnumerable<CategoryViewModel> Map(IEnumerable<Category> categories)
        {
            return categories.Select(Map);
        }
    }

    public class QuestionViewModel : EntityViewModel
    {
        public string Body { get; set; }
        public int Difficulty { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswerNumber { get; set; }

        public static QuestionViewModel Map(Question question)
        {
            return Mapper.Map<QuestionViewModel>(question);
        }
    }
}