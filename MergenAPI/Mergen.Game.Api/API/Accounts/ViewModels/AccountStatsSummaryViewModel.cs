using Mergen.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mergen.Game.Api.API.Accounts.ViewModels
{
    public class AccountStatsSummaryViewModel
    {
        public long AccountId { get; set; }
        public int Level { get; set; }
        public int Sky { get; set; }
        public int Rank { get; set; }
        public decimal Score { get; set; }
        public decimal Coins { get; set; }
        public int TotalBattlesPlayed { get; set; }
        public int WinCount { get; set; }
        public double WinRatio { get; set; }
        public int LoseCount { get; set; }
        public double LoseRatio { get; set; }
        public int AceWinCount { get; set; }
        public int ContinuousActiveDaysCount { get; set; }
        public int ContinuousActiveDaysRecord { get; set; }
        public DateTime? LastPlayDateTime { get; set; }
        public int PurchasedItemsCount { get; set; }
        public int InvitedPlayersCount { get; set; }
        public decimal GiftedCoins { get; set; }
        public int UnlockedAchievements { get; set; }
        public IEnumerable<AccountCategoryStatViewModel> Top5CategoryStats { get; set; }

        public static AccountStatsSummaryViewModel Map(AccountStatsSummary stats, List<AccountCategoryStat> categoryStats)
        {
            var model = AutoMapper.Mapper.Map<AccountStatsSummaryViewModel>(stats);
            model.Top5CategoryStats = categoryStats.Select(AccountCategoryStatViewModel.Map);
            return model;
        }

        public static AccountStatsSummaryViewModel Map(AccountStatsSummary stats, List<AccountCategoryStatViewModel> categoryStats)
        {
            var model = AutoMapper.Mapper.Map<AccountStatsSummaryViewModel>(stats);
            model.Top5CategoryStats = categoryStats;
            return model;
        }
    }
}