using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Options;
using Mergen.Game.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Mergen.Game.Api.API.Battles
{
    public class BattleController : ApiControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly GamingService _gamingService;
        private readonly BattleMapper _battleMapper;
        private readonly GameSettings _gameSettingsOptions;

        public BattleController(DataContext dataContext, GamingService gamingService, BattleMapper battleMapper, IOptions<GameSettings> gameSettingsOptions)
        {
            _dataContext = dataContext;
            _gamingService = gamingService;
            _battleMapper = battleMapper;
            _gameSettingsOptions = gameSettingsOptions.Value;
        }

        [HttpGet]
        [Route("accounts/{accountId}/activebattles")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<OneToOneBattleViewModel>>>> GetActiveBattles([Required]long accountId, CancellationToken cancellationToken)
        {
            var activeBattles = await _dataContext.OneToOneBattles.AsNoTracking()
                .Where(q => (q.Player1Id == accountId || q.Player2Id == accountId) && q.BattleStateId != BattleStateIds.Completed && q.BattleStateId != BattleStateIds.Expired)
                .ToListAsync(cancellationToken);

            return OkData(await _battleMapper.MapAllAsync(activeBattles, cancellationToken));
        }

        [HttpGet]
        [Route("accounts/{accountId}/recentbattles")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<BattleViewModel>>>> GetRecentBattles(long accountId, CancellationToken cancellationToken)
        {
            var recentBattles = await _dataContext.OneToOneBattles
                .Where(q => (q.Player1Id == accountId || q.Player2Id == accountId) &&
                            (q.BattleStateId == BattleStateIds.Completed || q.BattleStateId == BattleStateIds.Expired))
                .ToListAsync(cancellationToken);

            return OkData(await _battleMapper.MapAllAsync(recentBattles, cancellationToken));
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

        [HttpPost]
        [Route("games/{gameId}/selectedCategory")]
        public async Task<ActionResult<ApiResultViewModel<GameViewModel>>> SelectCategory(long gameId, long categoryId, bool customCategory, CancellationToken cancellationToken)
        {
            var game = await _dataContext.Games.Include(q=>q.GameCategories).ThenInclude(q=>q.Category).FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);
            if (game == null)
                return NotFound();

            if (game.SelectedCategoryId != null)
                return BadRequest("already_selected");

            using (var trans = new TransactionScope())
            {
                Category category;
                if (customCategory)
                {
                    category = await _dataContext.Categories.FirstOrDefaultAsync(q => q.Id == categoryId, cancellationToken);
                    if (category == null)
                        return BadRequest();

                    var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q =>
                        q.AccountId == game.PlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);

                    if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.SelectCustomCategoryPrice)
                        return BadRequest("insufficient_funds",
                            $"You must have {_gameSettingsOptions.SelectCustomCategoryPrice} coins to select custom category.");

                    accountCoin.Quantity -= _gameSettingsOptions.SelectCustomCategoryPrice;
                }
                else
                {
                    category = game.GameCategories.FirstOrDefault(q => q.CategoryId == categoryId)?.Category;
                    if (category == null)
                        return BadRequest();
                }

                game.SelectedCategoryId = categoryId;
                await _dataContext.SaveChangesAsync(cancellationToken);

                trans.Complete();
            }

            return OkData(GameViewModel.Map(game));
        }

        [HttpPost]
        [Route("games/{gameId}/randomizecategories")]
        public async Task<ActionResult<ApiResultViewModel<GameViewModel>>> RandomizeGameCategories(long gameId,
            CancellationToken cancellationToken)
        {
            using (var tran = new TransactionScope())
            {
                var game = await _dataContext.Games.Include(q => q.GameCategories).ThenInclude(q => q.Category).FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);
                if (game == null)
                    return NotFound();

                var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q =>
                    q.AccountId == game.PlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);

                if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.RandomizeCategoryPrice)
                    return BadRequest("insufficient_funds",
                        $"You must have {_gameSettingsOptions.RandomizeCategoryPrice} coins to select custom category.");

                accountCoin.Quantity -= _gameSettingsOptions.RandomizeCategoryPrice;

                await _gamingService.RandomizeCategories(game, cancellationToken);

                tran.Complete();
            }

            return Ok();
        }


        [HttpGet]
        [Route("activecategories")]
        public async Task<ActionResult<ApiResultViewModel<CategoryViewModel>>> GetActiveCategories(
            CancellationToken cancellationToken)
        {
            var activeCategories = await _dataContext.Categories.AsNoTracking()
                .Where(q => q.StatusId == CategoryStatusIds.Enabled).ToListAsync(cancellationToken);
            return OkData(CategoryViewModel.Map(activeCategories));
        }

        

        /*[HttpPost]
        [Route("accounts/{accountId}/onetoonebattleinvitations")]
        public async Task<ApiResultViewModel<OneToOneBattle>> InvitePlayerToOneToOneBattle(
            CancellationToken cancellationToken)
        {

        }*/
    }
}