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

        /*        [HttpPost]
                [Route("battles/{battleId}/games")]
                public async Task<ActionResult<ApiResultViewModel<GameViewModel>>> NewGame(long battleId, long categoryId,
                    bool customCategory, CancellationToken cancellationToken)
                {
                    var battle = await _dataContext.OneToOneBattles
                        .Include(q => q.LastGame)
                        .FirstOrDefaultAsync(q => q.Id == battleId, cancellationToken);

                    if (battle is null)
                        return NotFound();

                    if (battle.BattleStateId != BattleStateIds.SelectCategory)
                        return BadRequest("invalid_battleState");

                    if (battle.LastGame is null)

                }*/

        [HttpPost]
        [Route("games/{gameId}/selectedCategory")]
        public async Task<ActionResult<ApiResultViewModel<GameViewModel>>> SelectCategory(long gameId, long categoryId, bool customCategory, CancellationToken cancellationToken)
        {
            var game = await _dataContext.Games
                .Include(q => q.Battle)
                .Include(q => q.GameCategories).ThenInclude(q => q.Category)
                .FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);
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
                        q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);

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

                game.GameState = game.CurrentTurnPlayerId == ((OneToOneBattle)game.Battle).Player1Id ? GameState.Player1AnswerQuestions : GameState.Player2AnswerQuestions;
                game.SelectedCategoryId = categoryId;

                // add random questions to battle
                var questions = await _dataContext.QuestionCategories.Where(q => q.CategoryId == categoryId).OrderBy(r => Guid.NewGuid()).Take(3).ToListAsync(cancellationToken);
                var gameQuestions = questions.Select(q => new GameQuestion
                {
                    GameId = game.Id,
                    QuestionId = q.QuestionId
                });
                game.GameQuestions = gameQuestions.ToList();

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
                    q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);

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

        [HttpPost]
        [Route("games/{gameId}/answers")]
        public async Task<ActionResult> AnswerGameQuestions([FromRoute]long gameId, [FromBody]AnswerGameQuestionsInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var game = await _dataContext.Games
                .Include(q => q.GameQuestions)
                .ThenInclude(q => q.Question)
                .ThenInclude(q => q.QuestionCategories)
                .FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);

            if (game == null)
                return NotFound();

            /*            if (game.CurrentTurnPlayerId != playerId)
                            return BadRequest("invalid_turn");*/

            var playerStat = await _dataContext.AccountStatsSummaries.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId, cancellationToken);
            if (playerStat == null)
            {
                playerStat = new AccountStatsSummary
                {
                    AccountId = game.CurrentTurnPlayerId.Value
                };

                _dataContext.AccountStatsSummaries.Add(playerStat);
            }

            if (inputModel.Answers.Count() != game.GameQuestions.Count)
                return BadRequest("invalid_answersCount");

            foreach (var answer in inputModel.Answers)
            {
                var gq = game.GameQuestions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);

                if (gq == null)
                    return BadRequest("invalid_answers", "no such question in this game.");

                var selectedAnswer = answer.SelectedAnswer;

                if (game.GameState == GameState.Player1AnswerQuestions)
                {
                    if (gq.Player1SelectedAnswer.HasValue)
                        return BadRequest("invalid_answers", "question already answered.");

                    gq.Player1SelectedAnswer = selectedAnswer;
                }
                else
                {
                    if (gq.Player2SelectedAnswer.HasValue)
                        return BadRequest("invalid_answers", "question already answered.");

                    gq.Player2SelectedAnswer = selectedAnswer;
                }

                if (gq.Question.CorrectAnswerNumber == selectedAnswer)
                {
                    gq.Score = 1;
                    game.Score += 1;
                }
                else
                    gq.Score = 0;

                switch (selectedAnswer)
                {
                    case 1:
                        gq.Question.Answer1ChooseHistory++;
                        break;
                    case 2:
                        gq.Question.Answer2ChooseHistory++;
                        break;
                    case 3:
                        gq.Question.Answer3ChooseHistory++;
                        break;
                    case 4:
                        gq.Question.Answer4ChooseHistory++;
                        break;
                }


                // Process Category stats for player
                foreach (var category in gq.Question.QuestionCategories)
                {
                    var playerCategoryStat = await _dataContext.AccountCategoryStats.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.CategoryId == category.Id, cancellationToken);
                    if (playerCategoryStat == null)
                    {
                        playerCategoryStat = new AccountCategoryStat
                        {
                            AccountId = game.CurrentTurnPlayerId.Value,
                            CategoryId = category.Id
                        };

                        _dataContext.AccountCategoryStats.Add(playerCategoryStat);
                    }

                    playerCategoryStat.TotalQuestionsCount += 1;
                    playerCategoryStat.CorrectAnswersCount += gq.Score;
                }

                if (answer.UsedAnswersHistoryHelper)
                {
                    var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);
                    if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.AnswersHistoryHelperPrice)
                    {
                        // TODO: cheat detected
                        return BadRequest("insufficient_funds", "insufficient funds to use this helper");
                    }

                    accountCoin.Quantity -= _gameSettingsOptions.AnswersHistoryHelperPrice;
                }

                if (answer.UsedRemoveTwoAnswersHelper)
                {
                    var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);
                    if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.RemoveTwoAnswersHelperPrice)
                    {
                        // TODO: cheat detected
                        return BadRequest("insufficient_funds", "insufficient funds to use this helper");
                    }

                    accountCoin.Quantity -= _gameSettingsOptions.RemoveTwoAnswersHelperPrice;
                }

                if (answer.UsedAskMergenHelper)
                {
                    var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);
                    if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.AskMergenHelperPrice)
                    {
                        // TODO: cheat detected
                        return BadRequest("insufficient_funds", "insufficient funds to use this helper");
                    }

                    accountCoin.Quantity -= _gameSettingsOptions.AskMergenHelperPrice;
                }
            }

            if (game.GameQuestions.All(q => q.Player1SelectedAnswer.HasValue && q.Player2SelectedAnswer.HasValue))
            {
                game.GameState = GameState.Completed;

                var battle = await _dataContext.OneToOneBattles
                    .Include(q => q.Games)
                    .Include(q => q.Player1)
                    .Include(q => q.Player2)
                    .FirstOrDefaultAsync(q => q.Id == game.BattleId, cancellationToken);

                if (battle.Games.Count == 5 && battle.Games.All(q => q.GameState == GameState.Completed))
                {
                    battle.BattleStateId = BattleStateIds.Completed;
                    int player1Score = 0;
                    int player2Score = 0;
                    foreach (var battleGame in battle.Games)
                    {
                        if (battleGame.CurrentTurnPlayerId == battle.Player1Id)
                        {
                            player1Score += battleGame.Score;
                        }
                        else
                        {
                            player2Score += battleGame.Score;
                        }
                    }

                    battle.Player1CorrectAnswersCount = player1Score;
                    battle.Player2CorrectAnswersCount = player2Score;
                }
                else
                {
                    var nextPlayer = game.CurrentTurnPlayerId == battle.Player1Id ? battle.Player2 : battle.Player1;
                    var newGame = await _gamingService.CreateGameAsync(battle, nextPlayer, cancellationToken);
                    battle.LastGame = newGame;
                }
            }
            else
            {
                var battle = await _dataContext.OneToOneBattles
                    .Include(q => q.Player1)
                    .Include(q => q.Player2)
                    .FirstOrDefaultAsync(q => q.Id == game.BattleId, cancellationToken);

                var nextPlayer = game.CurrentTurnPlayerId == battle.Player1Id ? battle.Player2 : battle.Player1;
                game.CurrentTurnPlayerId = nextPlayer?.Id;
                game.GameState = game.CurrentTurnPlayerId == battle.Player1Id ? GameState.Player1AnswerQuestions : GameState.Player2AnswerQuestions;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        /*[HttpPost]
        [Route("accounts/{accountId}/onetoonebattleinvitations")]
        public async Task<ApiResultViewModel<OneToOneBattle>> InvitePlayerToOneToOneBattle(
            CancellationToken cancellationToken)
        {

        }*/
    }
}