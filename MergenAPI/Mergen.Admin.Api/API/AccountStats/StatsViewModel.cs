using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.AccountStats
{
    public class StatsViewModel
    {
        public string AccountId { get; set; }
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
        public int TotalGroupGamesCount { get; set; }
        public int AceWinCount { get; set; }
        public int ContinuousActiveDaysCount { get; set; }
        public int ContinuousActiveDaysRecord { get; set; }
        public DateTime? LastPlayDateTime { get; set; }
        public int PurchasedItemsCount { get; set; }
        public int InvitedPlayersCount { get; set; }
        public decimal GiftedCoins { get; set; }
        public int UnlockedAchievements { get; set; }
        public string Top3Skills { get; set; }

        public static StatsViewModel Map(AccountStatsSummary accountStatsSummary)
        {
            return Mapper.Map<StatsViewModel>(accountStatsSummary);
        }

        public static IEnumerable<StatsViewModel> MapAll(IEnumerable<AccountStatsSummary> accountStatsSummaries)
        {
            return accountStatsSummaries.Select(Map);
        }
    }
}