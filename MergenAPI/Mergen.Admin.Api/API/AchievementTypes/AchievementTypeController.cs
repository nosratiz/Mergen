using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Api.Core.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Admin.Api.API.AchievementTypes
{
    public class AchievementTypeController : ApiControllerBase
    {
        private readonly AchievementTypeManager _achievementTypeManager;

        public AchievementTypeController(AchievementTypeManager achievementTypeManager)
        {
            _achievementTypeManager = achievementTypeManager;
        }

        [Route("achievementtypes")]
        [HttpGet]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<AchievementTypeViewModel>>>>
            GetAchievementTypesAsync(QueryInputModel<AchievementTypeFilterInputModel> filter,
                CancellationToken cancellationToken)
        {
            var result = await _achievementTypeManager.GetAllAsync(filter, cancellationToken);
            return OkData(AchievementTypeViewModel.MapAll(result.Data), result.TotalCount);
        }

        [Route("achievementtypes/{id}")]
        [HttpGet]
        public async Task<ActionResult<ApiResultViewModel<AchievementTypeViewModel>>> GetAchievementTypeByIdAsync(
            [FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _achievementTypeManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            return OkData(AchievementTypeViewModel.Map(result));
        }

        [Route("achievementtypes/archive")]
        [HttpPost]
        public async Task<ActionResult> ArchiveAchievementType(
            [FromBody] ArchiveAchievementTypeInputModel input, CancellationToken cancellationToken)
        {
            var result = await _achievementTypeManager.GetByIdAsyncThrowNotFoundIfNotExists(input.AchievementTypeId, cancellationToken);
            await _achievementTypeManager.ArchiveAsync(result, cancellationToken);
            return Ok();
        }

        [Route("achievementtypes/{id}")]
        [HttpPut]
        public async Task<ActionResult<ApiResultViewModel<AchievementTypeViewModel>>> UpdateAchievementTypeByIdAsync(
            [FromRoute] string id, [FromBody] AchievementTypeInputModel input, CancellationToken cancellationToken)
        {
            var achievementType = await _achievementTypeManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);

            achievementType.CategoryId = input.CategoryId?.ToLong();
            achievementType.CorrectAnswersCountInCategory = input.CorrectAnswersCountInCategory;
            achievementType.WinnedBattlesCount = input.WinnedBattlesCount;
            achievementType.AceWinCount = input.AceWinCount;
            achievementType.NumberOfContinuousDaysPlaying = input.NumberOfContinuousDaysPlaying;
            achievementType.GiftedCoinsAmount = input.GiftedCoinsAmount;
            achievementType.NumberOfTotalBattlesPlayed = input.NumberOfTotalBattlesPlayed;
            achievementType.NumberOfRegisteredFriendsViaInviteLink = input.NumberOfRegisteredFriendsViaInviteLink;
            achievementType.NumberOfSuccessfulBattleInvitations = input.NumberOfSuccessfulBattleInvitations;
            achievementType.RemoveTwoAnswersHelperUsageCount = input.RemoveTwoAnswersHelperUsageCount;
            achievementType.AnswerHistoryHelperUsageCount = input.AnswerHistoryHelperUsageCount;
            achievementType.AskMergenHelperUsageCount = input.AskMergenHelperUsageCount;
            achievementType.DoubleChanceHelperUsageCount = input.DoubleChanceHelperUsageCount;
            achievementType.CoinsSpentOnAvatarItems = input.CoinsSpentOnAvatarItems;
            achievementType.CoinsSpentOnBooster = input.CoinsSpentOnBooster;
            achievementType = await _achievementTypeManager.SaveAsync(achievementType, cancellationToken);

            achievementType = await _achievementTypeManager.SaveAsync(achievementType, cancellationToken);
            return OkData(AchievementTypeViewModel.Map(achievementType));
        }

        [Route("achievementtypes/{id}")]
        [HttpDelete]
        public async Task<ActionResult<ApiResultViewModel<AchievementTypeViewModel>>> DeleteAchievementTypeByIdAsync(
            [FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _achievementTypeManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            await _achievementTypeManager.DeleteAsync(result, cancellationToken);
            return Ok();
        }

        [Route("achievementtypes")]
        [HttpPost]
        public async Task<ActionResult<ApiResultViewModel<AchievementTypeViewModel>>> CreateAchievementTypeAsync(
            [FromBody] AchievementTypeInputModel input, CancellationToken cancellationToken)
        {
            var achievementType = new AchievementType();
            achievementType.CategoryId = input.CategoryId?.ToLong();
            achievementType.CorrectAnswersCountInCategory = input.CorrectAnswersCountInCategory;
            achievementType.WinnedBattlesCount = input.WinnedBattlesCount;
            achievementType.AceWinCount = input.AceWinCount;
            achievementType.NumberOfContinuousDaysPlaying = input.NumberOfContinuousDaysPlaying;
            achievementType.GiftedCoinsAmount = input.GiftedCoinsAmount;
            achievementType.NumberOfTotalBattlesPlayed = input.NumberOfTotalBattlesPlayed;
            achievementType.NumberOfRegisteredFriendsViaInviteLink = input.NumberOfRegisteredFriendsViaInviteLink;
            achievementType.NumberOfSuccessfulBattleInvitations = input.NumberOfSuccessfulBattleInvitations;
            achievementType.RemoveTwoAnswersHelperUsageCount = input.RemoveTwoAnswersHelperUsageCount;
            achievementType.AnswerHistoryHelperUsageCount = input.AnswerHistoryHelperUsageCount;
            achievementType.AskMergenHelperUsageCount = input.AskMergenHelperUsageCount;
            achievementType.DoubleChanceHelperUsageCount = input.DoubleChanceHelperUsageCount;
            achievementType.CoinsSpentOnAvatarItems = input.CoinsSpentOnAvatarItems;
            achievementType.CoinsSpentOnBooster = input.CoinsSpentOnBooster;
            achievementType = await _achievementTypeManager.SaveAsync(achievementType, cancellationToken);

            return OkData(AchievementTypeViewModel.Map(achievementType));
        }
    }
}