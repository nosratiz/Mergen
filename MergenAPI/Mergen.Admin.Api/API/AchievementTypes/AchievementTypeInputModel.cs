using System;
using System.ComponentModel.DataAnnotations;

namespace Mergen.Admin.Api.API.AchievementTypes
{
    public class AchievementTypeInputModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageFileId { get; set; }

        public string CategoryId { get; set; }
        public long? CorrectAnswersCountInCategory { get; set; }

        public long? WinnedBattlesCount { get; set; }
        public long? AceWinCount { get; set; }
        public long? NumberOfContinuousDaysPlaying { get; set; }
        public long? GiftedCoinsAmount { get; set; }
        public long? NumberOfTotalBattlesPlayed { get; set; }
        public long? NumberOfRegisteredFriendsViaInviteLink { get; set; }
        public long? NumberOfSuccessfulBattleInvitations { get; set; }
        public long? RemoveTwoAnswersHelperUsageCount { get; set; }
        public long? AnswerHistoryHelperUsageCount { get; set; }
        public long? AskMergenHelperUsageCount { get; set; }
        public long? DoubleChanceHelperUsageCount { get; set; }
        public long? CoinsSpentOnAvatarItems { get; set; }
        public long? CoinsSpentOnBooster { get; set; }
    }
}
