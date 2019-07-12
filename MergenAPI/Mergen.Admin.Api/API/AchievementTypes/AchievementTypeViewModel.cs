using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.AchievementTypes
{
    public class AchievementTypeViewModel : EntityViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageFileId { get; set; }

        public string CategoryId { get; set; }
        public string CorrectAnswersCountInCategory { get; set; }

        public string WinnedBattlesCount { get; set; }
        public string AceWinCount { get; set; }
        public string NumberOfContinuousDaysPlaying { get; set; }
        public string GiftedCoinsAmount { get; set; }
        public string NumberOfTotalBattlesPlayed { get; set; }
        public string NumberOfRegisteredFriendsViaInviteLink { get; set; }
        public string NumberOfSuccessfulBattleInvitations { get; set; }
        public string RemoveTwoAnswersHelperUsageCount { get; set; }
        public string AnswerHistoryHelperUsageCount { get; set; }
        public string AskMergenHelperUsageCount { get; set; }
        public string DoubleChanceHelperUsageCount { get; set; }
        public string CoinsSpentOnAvatarItems { get; set; }
        public string CoinsSpentOnBooster { get; set; }

        public static AchievementTypeViewModel Map(AchievementType achievementType)
        {
            return AutoMapper.Mapper.Map<AchievementTypeViewModel>(achievementType);
        }

        public static IEnumerable<AchievementTypeViewModel> MapAll(IEnumerable<AchievementType> achievementTypes)
        {
            return achievementTypes.Select(Map);
        }
    }
}
