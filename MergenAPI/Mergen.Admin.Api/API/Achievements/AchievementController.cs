using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Api.Core.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.Achievements
{
    [ApiController]
    public class AchievementController : ApiControllerBase
    {
        private readonly AchievementManager _achievementManager;

        public AchievementController(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        [HttpGet]
        [Route("achievements")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<AchievementViewModel>>>> GetAll([FromQuery] QueryInputModel<AchievementsFilterInputModel> query, CancellationToken cancellationToken)
        {
            var data = await _achievementManager.GetAllAsync(query, cancellationToken);
            return OkData(AchievementViewModel.MapAll(data.Data), new DataMetaViewModel(data.TotalCount));
        }

        [HttpPost]
        [Route("accounts/{accountId}/achievements")]
        public async Task<ActionResult<ApiResultViewModel<AchievementViewModel>>> Add([FromRoute] string accountId, [FromBody] AchievementInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var achievementTypeId = inputModel.AchievementTypeId.ToLong();
            var existingAchievement = await _achievementManager.GetAsync(accountId.ToLong(),
                achievementTypeId,
                cancellationToken);

            if (existingAchievement != null)
                return BadRequest("duplicate_achievement", "Player already has this achievement.");

            var achievement = new Achievement
            {
                AccountId = accountId.ToLong(),
                AchievementTypeId = achievementTypeId,
                AchieveDateTime = DateTime.UtcNow
            };

            achievement = await _achievementManager.SaveAsync(achievement, cancellationToken);
            return OkData(AchievementViewModel.Map(achievement));
        }

        [HttpDelete]
        [Route("accounts/{accountId}/achievements")]
        public async Task<ActionResult> Delete([FromRoute] string accountId, [FromQuery] string achievementTypeId, CancellationToken cancellationToken)
        {
            var achievement =
                await _achievementManager.GetAsync(accountId.ToLong(), achievementTypeId.ToLong(), cancellationToken);

            if (achievement == null)
                return NotFound();

            await _achievementManager.DeleteAsync(achievement, cancellationToken);

            return Ok();
        }
    }
}