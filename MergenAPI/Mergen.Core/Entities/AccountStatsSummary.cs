using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountStatsSummary : Entity
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
        public string Top3Skills { get; set; }
        public int SuccessfulBattleInvitationsCount { get; set; }
    }
}