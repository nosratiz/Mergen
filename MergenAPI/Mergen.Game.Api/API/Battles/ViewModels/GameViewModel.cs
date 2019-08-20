using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;

namespace Mergen.Game.Api.API.Battles.ViewModels
{
    public class GameViewModel : EntityViewModel
    {
        public CategoryViewModel SelectedCategory { get; set; }
        public IEnumerable<CategoryViewModel> AvailableCategories { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; }
        public int CurrentTurnPlayerId { get; set; }
        public GameStateIds GameState { get; set; }

        public static GameViewModel Map(Core.Entities.Game game)
        {
            var vm = Mapper.Map<GameViewModel>(game);
            vm.AvailableCategories = game.GameCategories.Select(q => CategoryViewModel.Map(q.Category));
            vm.Questions = game.GameQuestions.Where(q => q.Question != null).Select(q =>
            {
                var model = QuestionViewModel.Map(q.Question);
                model.Player1SelectedAnswer = q.Player1SelectedAnswer;
                model.Player2SelectedAnswer = q.Player2SelectedAnswer;
                return model;
            }).ToList();
            return vm;
        }

        public static IEnumerable<GameViewModel> Map(IEnumerable<Core.Entities.Game> games)
        {
            return games.Select(Map);
        }
    }

    public class CategoryViewModel : EntityViewModel
    {
        public string Title { get; set; }
        public string IconFileId { get; set; }
        public string CoverImageFileId { get; set; }
        public string Description { get; set; }

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
        public long Answer1ChooseHistory { get; set; }
        public string Answer2 { get; set; }
        public long Answer2ChooseHistory { get; set; }
        public string Answer3 { get; set; }
        public long Answer3ChooseHistory { get; set; }
        public string Answer4 { get; set; }
        public long Answer4ChooseHistory { get; set; }
        public int CorrectAnswerNumber { get; set; }
        public int? Player1SelectedAnswer { get; set; }
        public int? Player2SelectedAnswer { get; set; }

        public static QuestionViewModel Map(Question question)
        {
            return Mapper.Map<QuestionViewModel>(question);
        }
    }
}