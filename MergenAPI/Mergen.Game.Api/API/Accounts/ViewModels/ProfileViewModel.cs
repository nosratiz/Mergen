using Mergen.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Mergen.Game.Api.API.Accounts.ViewModels
{
    public class ProfileViewModel
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public int? Level { get; set; }
        public string AvatarImageId { get; set; }
        public int Rank { get; set; }

        public static ProfileViewModel Map(Account account, AccountStatsSummary stats)
        {
            return new ProfileViewModel
            {
                AccountId = account.Id.ToString(),
                Name = account.Nickname,
                Level = stats?.Level,
                AvatarImageId = account.AvatarImageId,
                Rank = stats?.Rank ?? 0
            };
        }

        public static IEnumerable<ProfileViewModel> Map(IEnumerable<(Account account, AccountStatsSummary stats)> accounts)
        {
            return accounts.Select(q => Map(q.account, q.stats));
        }
    }
}