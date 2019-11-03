using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Accounts.ViewModels
{
    public class AccountCategoryStatViewModel
    {
        public long CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public long TotalQuestionsCount { get; set; }
        public long CorrectAnswersCount { get; set; }

        public static AccountCategoryStatViewModel Map(AccountCategoryStat categoryStat)
        {
            var model = AutoMapper.Mapper.Map<AccountCategoryStatViewModel>(categoryStat);
            model.CategoryTitle = categoryStat.Category?.Title;
            return model;
        }
    }
}