using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AchievementType : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageFileId { get; set; }

        public long? CategoryId { get; set; }
        public long? CorrectAnswersCountInCategory { get; set; }

        public long? WinnedBattlesCount { get; set; }
        public long? AceWinCount { get; set; }
        public long? NumberOfContinuousDaysPlaying { get; set; }
        public long? GiftedCoinsAmount { get; set; }
        public long? NumberOfTotalBattlesPlayed { get; set; }
        public long? NumberOfRegisteredFriendsViaInviteLink { get; set; }
        public long? NumberOfInvitesThatAccepted { get; set; }
        public long? RemoveTwoAnswersHelperUsageCount { get; set; }
        public long? AnswerHistoryHelperUsageCount { get; set; }
        public long? AskMergenHelperUsageCount { get; set; }
        public long? DoubleChanceHelperUsageCount { get; set; }
        public long? CoinsSpentOnAvatarItems { get; set; }
        public long? CoinsSpentOnBooster { get; set; }
    }
}