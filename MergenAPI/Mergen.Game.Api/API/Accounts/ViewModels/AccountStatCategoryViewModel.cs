using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Accounts.ViewModels
{
    public class AccountStatCategoryViewModel
    {
        public long CategoryId { get; set; }

        public AccountCategoryStat AccountCategoryStat { get; set; }
    }
}