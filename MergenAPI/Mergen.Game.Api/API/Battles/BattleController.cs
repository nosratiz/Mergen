using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Game.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Game.Api.API.Battles
{
    public class BattleController : ApiControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly GamingService _gamingService;
        private readonly BattleMapper _battleMapper;

        public BattleController(DataContext dataContext, GamingService gamingService, BattleMapper battleMapper)
        {
            _dataContext = dataContext;
            _gamingService = gamingService;
            _battleMapper = battleMapper;
        }

        [HttpGet]
        [Route("accounts/{accountId}/activebattles")]
        public async Task<ApiResultViewModel<BattleViewModel>> GetActiveBattles([Required]string accountId, CancellationToken cancellationToken)
        {
            return null;
        }

        [HttpGet]
        [Route("accounts/{accountId}/recentbattles")]
        public async Task<ApiResultViewModel<BattleViewModel>> GetRecentBattles(string accountId, CancellationToken cancellationToken)
        {

            return null;
        }

        [HttpPost]
        [Route("accounts/{accountId}/onetoonebattles")]
        public async Task<ActionResult<ApiResultViewModel<OneToOneBattleViewModel>>> StartRandomBattle(long accountId, CancellationToken cancellationToken)
        {
            var player1 = await _dataContext.Accounts.FirstOrDefaultAsync(q => q.Id == accountId, cancellationToken);

            var battle = await _gamingService.StartRandomBattleAsync(player1, null, cancellationToken);

            return OkData(await _battleMapper.MapAsync(battle, cancellationToken));
        }

        [HttpGet]
        [Route("battles/{battleId}")]
        public async Task<ActionResult<ApiResultViewModel<BattleViewModel>>> GetBattleById(long battleId, CancellationToken cancellationToken)
        {
            var battle = await _dataContext.OneToOneBattles.FirstOrDefaultAsync(q => q.Id == battleId, cancellationToken);
            return OkData(await _battleMapper.MapAsync(battle, cancellationToken));
        }

        [HttpGet]
        [Route("battles/{battleId}/games")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<GameViewModel>>>> GetBattleGamesByBattleId(long battleId,
            CancellationToken cancellationToken)
        {
            var battle = await _dataContext.OneToOneBattles
                .Include(q => q.Games).ThenInclude(q => q.GameCategories).ThenInclude(q => q.Category)
                .Include(q => q.Games).ThenInclude(q => q.GameQuestions).ThenInclude(q => q.Question)
                .FirstOrDefaultAsync(q => q.Id == battleId, cancellationToken);

            return OkData(GameViewModel.Map(battle.Games));
        }

        [HttpGet]
        [Route("games/{gameId}")]
        public async Task<ActionResult<ApiResultViewModel<GameViewModel>>> GetGameById(long gameId,
            CancellationToken cancellationToken)
        {
            var game = await _dataContext.Games
                .Include(q => q.GameCategories).ThenInclude(q => q.Category)
                .Include(q => q.GameQuestions).ThenInclude(q => q.Question)
                .FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);

            return OkData(GameViewModel.Map(game));
        }

        /*[HttpPost]
        [Route("accounts/{accountId}/onetoonebattleinvitations")]
        public async Task<ApiResultViewModel<OneToOneBattle>> InvitePlayerToOneToOneBattle(
            CancellationToken cancellationToken)
        {

        }*/
    }
}