using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Accounts.ViewModels
{
    public class AccountCategoryStatViewModel
    {
        public string CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int TotalQuestionsCount { get; set; }
        public int CorrectAnswersCount { get; set; }

        public static AccountCategoryStatViewModel Map(AccountCategoryStat categoryStat)
        {
            var model = AutoMapper.Mapper.Map<AccountCategoryStatViewModel>(categoryStat);
            model.CategoryTitle = categoryStat.Category?.Title;
            return model;
        }
    }
}