using Mergen.Api.Core.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.GameServices;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Mergen.Game.Api.API.Battles.InputModels;
using Mergen.Game.Api.API.Battles.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Mergen.Game.Api.API.Accounts.ViewModels;

namespace Mergen.Game.Api.API.Battles
{
    public class BattleController : ApiControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly GamingService _gamingService;
        private readonly BattleMapper _battleMapper;
        private readonly LevelManager _levelManager;
        private readonly AchievementService _achievementService;
        private readonly GameSettings _gameSettingsOptions;
        private readonly NotificationManager _notificationManager;
        private readonly AccountManager _accountManager;
        private readonly BattleManager _battleManager;
        private readonly StatsManager _statsManager;

        private const int ExperienceBase = 10;
        private const int WinExperienceMultiplier = 3;
        private const int LoseExperienceMultiplier = 1;
        private const int DrawExperienceMultiplier = 2;

        private const int CoinBase = 10;
        private const int WinCoinMultiplier = 2;
        private const int LoseCoinMultiplier = 1;

        public BattleController(DataContext dataContext, GamingService gamingService, BattleMapper battleMapper, IOptions<GameSettings> gameSettingsOptions, LevelManager levelManager, AchievementService achievementService, NotificationManager notificationManager, AccountManager accountManager, BattleManager battleManager)
        {
            _dataContext = dataContext;
            _gamingService = gamingService;
            _battleMapper = battleMapper;
            _levelManager = levelManager;
            _achievementService = achievementService;
            _notificationManager = notificationManager;
            _accountManager = accountManager;
            _battleManager = battleManager;
            _gameSettingsOptions = gameSettingsOptions.Value;
        }

        [HttpGet]
        [Route("accounts/{accountId}/activebattles")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<OneToOneBattleViewModel>>>> GetActiveBattles([Required]long accountId, CancellationToken cancellationToken)
        {
            var activeBattles = await _dataContext.OneToOneBattles.AsNoTracking()
                .Where(q => q.IsArchived == false && (q.Player1Id == accountId || q.Player2Id == accountId) && q.BattleStateId != BattleStateIds.Completed && q.BattleStateId != BattleStateIds.Expired)
                .Include(x => x.Games).ThenInclude(x => x.GameQuestions).ThenInclude(x => x.Question).ToListAsync(cancellationToken);


            return OkData(await _battleMapper.MapAllAsync(activeBattles, cancellationToken));
        }

        [HttpGet]
        [Route("accounts/{accountId}/recentbattles")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<OneToOneBattleViewModel>>>> GetRecentBattles(long accountId, CancellationToken cancellationToken)
        {
            var recentBattles = await _dataContext.OneToOneBattles
                .Where(q => q.IsArchived == false && (q.Player1Id == accountId || q.Player2Id == accountId) &&
                            (q.BattleStateId == BattleStateIds.Completed || q.BattleStateId == BattleStateIds.Expired))
                .ToListAsync(cancellationToken);

            return OkData(await _battleMapper.MapAllAsync(recentBattles, cancellationToken));
        }

        [HttpPost]
        [Route("accounts/{accountId}/onetoonebattles")]
        public async Task<ActionResult<ApiResultViewModel<OneToOneBattleViewModel>>> StartRandomBattle([FromRoute] long accountId, [FromBody] OneToOneBattleInputModel inputModel, CancellationToken cancellationToken)
        {
            var currentPlayer = await _dataContext.Accounts.FirstOrDefaultAsync(q => q.Id == accountId, cancellationToken);

            if (currentPlayer == null)
                return BadRequest("invalid_accountId", "Player1 account not found");

            var activebattle = await _dataContext.OneToOneBattles.Where(x =>
                x.IsArchived == false && x.WinnerPlayerId == null &&
                (x.Player1Id == accountId || x.Player2Id == accountId)).ToListAsync(cancellationToken: cancellationToken);

            if (activebattle.Count() > 5)
                return BadRequest("Too many Active battle", "you can just allow to have 5 active battle");



            OneToOneBattle battle;
            if (inputModel.BattleInvitationId != null)
            {
                var battleInvitation = await _dataContext.BattleInvitations.FirstOrDefaultAsync(q => q.Id == inputModel.BattleInvitationId, cancellationToken);
                if (battleInvitation == null)
                    return BadRequest("invalid_battleInvitationId", "Invitation not found.");

                battleInvitation.Status = BattleInvitationStatus.Accepted;


                var inviterPlayerStats = await _dataContext.AccountStatsSummaries.FirstAsync(q => q.IsArchived == false &&
                                                                                                  q.AccountId == battleInvitation.InviterAccountId, cancellationToken);
                inviterPlayerStats.SuccessfulBattleInvitationsCount += 1;

                var account = await _accountManager.GetAsync(accountId, cancellationToken);
                await _achievementService.ProcessSuccessfulBattleInvitationAchievements(battleInvitation, inviterPlayerStats, cancellationToken);

                await _notificationManager.SaveAsync(
                    new Core.Entities.Notification
                    {
                        AccountId = accountId,
                        Body = $"Accept Battle By  {account.Email}",
                        NotificationTypeId = NotificationTypeIds.General,
                        Title = "Accept battle"
                    }, cancellationToken);

                await _dataContext.SaveChangesAsync(cancellationToken);

                var inviterPlayer = await _dataContext.Accounts.FirstOrDefaultAsync(q => q.Id == battleInvitation.InviterAccountId, cancellationToken);
                battle = await _gamingService.StartRandomBattleAsync(inviterPlayer, currentPlayer, cancellationToken);
            }
            else
            {
                battle = await _gamingService.StartRandomBattleAsync(currentPlayer, null, cancellationToken);
            }

            return OkData(await _battleMapper.MapAsync(battle, cancellationToken));
        }

        [HttpGet]
        [Route("battles/{battleId}")]
        public async Task<ActionResult<ApiResultViewModel<OneToOneBattleViewModel>>> GetBattleById(long battleId, CancellationToken cancellationToken)
        {
            var battle = await _dataContext.OneToOneBattles.Include(x => x.Games)
                .FirstOrDefaultAsync(q => q.Id == battleId, cancellationToken);
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
                .Include(x => x.Games)
                .FirstOrDefaultAsync(q => q.Id == battleId, cancellationToken);

            if (battle == null)
                return NotFound();

            if (battle.Player1Id != AccountId && battle.Player2Id != AccountId)
                return Forbidden();

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
        public async Task<ActionResult<ApiResultViewModel<GameViewModel>>> SelectCategory([FromRoute]long gameId, [FromBody] SelectedCategoryInputModel input, CancellationToken cancellationToken)
        {
            var categoryId = input.CategoryId.ToLong();
            var customCategory = input.CustomCategory;

            var game = await _dataContext.Games
                .Include(q => q.Battle)
                .Include(q => q.GameCategories).ThenInclude(q => q.Category)
                .FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);

            if (game == null)
                return NotFound();

            if (game.CurrentTurnPlayerId != AccountId)
                return BadRequest("invalid_player_turn", "invalid player turn");

            if (game.SelectedCategoryId != null)
                return BadRequest("already_selected", "already selected");

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Category category;
                if (customCategory)
                {
                    category = await _dataContext.Categories.FirstOrDefaultAsync(q => q.Id == categoryId, cancellationToken);
                    if (category == null || category.StatusId != CategoryStatusIds.Enabled || category.IsArchived)
                        return BadRequest("category not found");

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
                        return BadRequest("category not found");
                }

                game.GameState = game.CurrentTurnPlayerId == ((OneToOneBattle)game.Battle).Player1Id ? GameStateIds.Player1AnswerQuestions : GameStateIds.Player2AnswerQuestions;
                game.SelectedCategoryId = categoryId;
                var battle = (OneToOneBattle)game.Battle;
                battle.BattleStateId = BattleStateIds.AnsweringQuestions;

                // add random questions to battle
                var questions = await _dataContext.QuestionCategories.Include(q => q.Question).Where(q => q.CategoryId == categoryId).OrderBy(r => Guid.NewGuid()).Take(3).ToListAsync(cancellationToken);
                var gameQuestions = questions.Select(q => new GameQuestion
                {
                    GameId = game.Id,
                    QuestionId = q.QuestionId,
                    Question = q.Question
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
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

                _dataContext.SaveChanges();

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
                .Where(q => q.IsArchived == false && q.StatusId == CategoryStatusIds.Enabled).ToListAsync(cancellationToken);
            return OkData(CategoryViewModel.Map(activeCategories));
        }

        [HttpPost]
        [Route("games/{gameId}/answers")]
        public async Task<ActionResult> AnswerGameQuestions([FromRoute]long gameId, [FromBody]AnswerGameQuestionsInputModel inputModel,
            CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var game = await _dataContext.Games
            .Include(q => q.GameQuestions)
            .ThenInclude(q => q.Question)
            .ThenInclude(q => q.QuestionCategories)
            .FirstOrDefaultAsync(q => q.Id == gameId, cancellationToken);

                #region Game Validation


                if (game == null)
                    return NotFound();

                if (game.GameState == GameStateIds.Completed || game.GameState == GameStateIds.SelectCategory)
                    return BadRequest("invalid_gameState", "invalid_gameState");

                if (game.CurrentTurnPlayerId != AccountId)
                    return BadRequest("invalid_turn", "invalid_turn");

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
                    return BadRequest("invalid_answersCount", "invalid_answersCount");

                #endregion


                var changedCategoryStats = new List<AccountCategoryStat>();

                foreach (var answer in inputModel.Answers)
                {
                    var gq = game.GameQuestions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);

                    if (gq == null)
                        return BadRequest("invalid_answers", "no such question in this game.");

                    var selectedAnswer = answer.SelectedAnswer;

                    if (game.GameState == GameStateIds.Player1AnswerQuestions)
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

                    var score = gq.Question.CorrectAnswerNumber == selectedAnswer ? 1 : 0;

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
                                CategoryId = category.CategoryId
                            };

                            _dataContext.AccountCategoryStats.Add(playerCategoryStat);
                            changedCategoryStats.Add(playerCategoryStat);
                        }

                        playerCategoryStat.TotalQuestionsCount += 1;
                        playerCategoryStat.CorrectAnswersCount += score;
                    }

                    if (answer.UsedAnswersHistoryHelper)
                    {
                        var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.ShopItemId == ShopItemTypeIds.Coin, cancellationToken);
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
                        playerStat.RemoveTwoAnswersHelperUsageCount += 1;
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
                        playerStat.AskMergenHelperUsageCount += 1;
                    }

                    if (answer.UsedDoubleChanceHelper)
                    {
                        var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);
                        if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.DoubleChanceHelperPrice)
                        {
                            // TODO: cheat detected
                            return BadRequest("insufficient_funds", "insufficient funds to use this helper");
                        }

                        accountCoin.Quantity -= _gameSettingsOptions.DoubleChanceHelperPrice;
                        playerStat.DoubleChanceHelperUsageCount += 1;
                    }

                    if (answer.TimeExtenderHelperUsageCount > 0)
                    {
                        var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);
                        if (accountCoin == null || accountCoin.Quantity < _gameSettingsOptions.TimeExtenderHelperPrice * answer.TimeExtenderHelperUsageCount)
                        {
                            // TODO: cheat detected
                            return BadRequest("insufficient_funds", "insufficient funds to use this helper");
                        }

                        accountCoin.Quantity -= _gameSettingsOptions.TimeExtenderHelperPrice * answer.TimeExtenderHelperUsageCount;
                        playerStat.TimeExtenderHelperUsageCount += 1;
                    }
                }

                await _achievementService.ProcessAnswerTimeAchievementsAsync(playerStat, changedCategoryStats, cancellationToken);

                var gameBattleId = game.BattleId;
                if (game.GameQuestions.All(q => q.Player1SelectedAnswer.HasValue && q.Player2SelectedAnswer.HasValue))
                {
                    game.GameState = GameStateIds.Completed;

                    var battle = await _dataContext.OneToOneBattles
                        .Include(q => q.Games).ThenInclude(q => q.GameQuestions).ThenInclude(q => q.Question)
                        .Include(q => q.Player1)
                        .Include(q => q.Player2)
                        .FirstOrDefaultAsync(q => q.Id == gameBattleId, cancellationToken);

                    if (battle.Games.Count !=6)
                        battle.Round += 1;      
                    
                  

                    if (battle.Games.Count == 6 && battle.Games.All(q => q.GameState == GameStateIds.Completed))
                    {
                        // Battle Completed

                        battle.BattleStateId = BattleStateIds.Completed;
                        int player1CorrectAnswersCount = 0;
                        int player2CorrectAnswersCount = 0;
                        foreach (var battleGame in battle.Games)
                        {
                            player1CorrectAnswersCount += battleGame.GameQuestions.Sum(q => q.Player1SelectedAnswer == q.Question.CorrectAnswerNumber ? 1 : 0);

                            player2CorrectAnswersCount += battleGame.GameQuestions.Sum(q => q.Player2SelectedAnswer == q.Question.CorrectAnswerNumber ? 1 : 0);
                        }

                        battle.Player1CorrectAnswersCount = player1CorrectAnswersCount;
                        battle.Player2CorrectAnswersCount = player2CorrectAnswersCount;

                        if (player1CorrectAnswersCount > player2CorrectAnswersCount)
                            battle.WinnerPlayerId = battle.Player1Id;

                        else if (player2CorrectAnswersCount > player1CorrectAnswersCount)
                            battle.WinnerPlayerId = battle.Player2Id;

                        var playersStats = await _dataContext.AccountStatsSummaries
                            .Where(q => q.AccountId == battle.Player1Id || q.AccountId == battle.Player2Id)
                            .ToDictionaryAsync(q => q.AccountId, q => q, cancellationToken);

                        var player1Stats = playersStats[battle.Player1Id];
                        var player2Stats = playersStats[battle.Player2Id.Value];

                        player1Stats.TotalBattlesPlayed += 1;
                        player2Stats.TotalBattlesPlayed += 1;

                        if (battle.WinnerPlayerId == battle.Player1Id)
                        {
                            player1Stats.WinCount += 1;
                            player2Stats.LoseCount += 1;
                            player1Stats.LastPlayDateTime = DateTime.Now;
                            player2Stats.LastPlayDateTime = DateTime.Now;

                            if (await _battleManager.HasPlayedYesterday(player1Stats.AccountId, cancellationToken))
                                player1Stats.ContinuousActiveDaysCount += 1;

                            if (await _battleManager.HasPlayedYesterday(player2Stats.AccountId, cancellationToken))
                                player2Stats.ContinuousActiveDaysCount += 1;


                            if (player1CorrectAnswersCount == 18)
                                player1Stats.AceWinCount += 1;
                            // Experience for win
                            player1Stats.Score += player1CorrectAnswersCount + ExperienceBase * WinExperienceMultiplier;
                            player1Stats.Coins += player1CorrectAnswersCount + CoinBase * WinCoinMultiplier;

                            // Experience for lose
                            player2Stats.Score += player2CorrectAnswersCount + ExperienceBase * LoseExperienceMultiplier;
                            player2Stats.Coins += player2CorrectAnswersCount + CoinBase * LoseCoinMultiplier;
                        }
                        else if (battle.WinnerPlayerId == battle.Player2Id)
                        {
                            player1Stats.LoseCount += 1;
                            player2Stats.WinCount += 1;

                            if (player2CorrectAnswersCount == 18)
                                player2Stats.AceWinCount += 1;

                            // Experience for win
                            player2Stats.Score += player2CorrectAnswersCount + ExperienceBase * WinExperienceMultiplier;
                            player2Stats.Coins += player2CorrectAnswersCount + CoinBase * WinCoinMultiplier;

                            // Experience for lose
                            player1Stats.Score += player1CorrectAnswersCount + ExperienceBase * LoseExperienceMultiplier;
                            player1Stats.Coins += player1CorrectAnswersCount + CoinBase * LoseCoinMultiplier;
                        }
                        else
                        {
                            // Exprience for draw (drop)

                            player1Stats.Score += player1CorrectAnswersCount + ExperienceBase * DrawExperienceMultiplier;
                            player2Stats.Score += player2CorrectAnswersCount + ExperienceBase * DrawExperienceMultiplier;
                        }

                        player1Stats.Level = _levelManager.GetLevel(player1Stats.Score)?.LevelNumber ?? 0;
                        player2Stats.Level = _levelManager.GetLevel(player2Stats.Score)?.LevelNumber ?? 0;

                        player1Stats.WinRatio = player1Stats.WinCount / (float)player1Stats.TotalBattlesPlayed;
                        player2Stats.WinRatio = player2Stats.WinCount / (float)player2Stats.TotalBattlesPlayed;

                        player1Stats.LoseRatio = player1Stats.LoseCount / (float)player1Stats.TotalBattlesPlayed;
                        player2Stats.LoseRatio = player2Stats.LoseCount / (float)player2Stats.TotalBattlesPlayed;

                        await _achievementService.ProcessBattleAchievementsAsync(battle, player1Stats, player2Stats,
                            cancellationToken);

                        var account = await _accountManager.GetAsync(game.CurrentTurnPlayerId.Value, cancellationToken);

                        var accountNotifId = game.CurrentTurnPlayerId == player1Stats.AccountId
                            ? player2Stats.AccountId
                            : player1Stats.AccountId;

                        await _notificationManager.SaveAsync(
                            new Core.Entities.Notification
                            {
                                AccountId = accountNotifId,
                                Body = $"Your battle Finish with{account.Email}",
                                NotificationTypeId = NotificationTypeIds.BattleCompleted,
                                Title = "Battle Completed"
                            }, cancellationToken);

                        await _dataContext.SaveChangesAsync(cancellationToken);

                        // Calculate Sky
                        player1Stats.Sky = await CalculatePlayerSkyAsync(battle.Player1Id, cancellationToken);
                        player2Stats.Sky = await CalculatePlayerSkyAsync(battle.Player2Id.Value, cancellationToken);
                    }

                    else
                    {
                        var nextPlayer = game.CurrentTurnPlayerId == battle.Player1Id ? battle.Player1 : battle.Player2;
                        var newGame = await _gamingService.CreateGameAsync(battle, nextPlayer.Id, cancellationToken);
                        battle.LastGame = newGame;
                    }
                }

                else
                {
                    var battle = await _dataContext.OneToOneBattles
                        .Include(q => q.Player1)
                        .Include(q => q.Player2)
                        .FirstOrDefaultAsync(q => q.Id == gameBattleId, cancellationToken);

                    var nextPlayer = game.CurrentTurnPlayerId == battle.Player1Id ? battle.Player2 : battle.Player1;
                    game.CurrentTurnPlayerId = nextPlayer?.Id;
                    game.GameState = game.CurrentTurnPlayerId == battle.Player1Id ? GameStateIds.Player1AnswerQuestions : GameStateIds.Player2AnswerQuestions;


                }

                await _dataContext.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            return Ok();
        }

        [HttpPost]
        [Route("accounts/{accountId}/battleinvitations")]
        public async Task<IActionResult> InvitePlayerToOneToOneBattle([FromRoute] long accountId, CancellationToken cancellationToken)
        {
            var battleInvitation = await _dataContext.BattleInvitations.FirstOrDefaultAsync(q => q.AccountId == accountId && q.InviterAccountId == AccountId, cancellationToken);
            if (battleInvitation != null)
                return BadRequest("duplicate_invitation", "Invitation already sent.");

            battleInvitation = new BattleInvitation
            {
                InviterAccountId = AccountId,
                AccountId = accountId,
                DateTime = DateTime.UtcNow,
                Status = BattleInvitationStatus.Pending
            };

            _dataContext.BattleInvitations.Add(battleInvitation);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("accounts/{accountId}/battleinvitations")]
        public async Task<ActionResult<ApiResultViewModel<BattleInvitationViewModel>>> GetBattleInvitations([FromRoute] long accountId, CancellationToken cancellationToken)
        {
            var invitations = await _dataContext.BattleInvitations
                .Include(q => q.InviterAccount)
                .Where(q => q.AccountId == accountId && q.Status == BattleInvitationStatus.Pending).ToListAsync(cancellationToken);

            return OkData(BattleInvitationViewModel.MapAll(invitations));
        }

        [HttpPost]
        [Route("battleinvitations/rejected")]
        public async Task<IActionResult> RejectBattleInvitation([FromBody] long battleInvitationId,
            CancellationToken cancellationToken)
        {
            return await ChangeBattleInvitationStatus(battleInvitationId, cancellationToken);
        }

        [HttpPost]
        [Route("battleinvitations/cancelled")]
        public async Task<IActionResult> CancelBattleInvitation([FromBody] long battleInvitationId,
            CancellationToken cancellationToken)
        {
            return await ChangeBattleInvitationStatus(battleInvitationId, cancellationToken);
        }

        [HttpPost]
        [Route("battleinvitations/ignored")]
        public async Task<IActionResult> IgnoreBattleInvitation([FromBody] long battleInvitationId,
            CancellationToken cancellationToken)
        {
            return await ChangeBattleInvitationStatus(battleInvitationId, cancellationToken);
        }


        [HttpGet]
        [Route("Battle/TopRank")]
        public async Task<IActionResult> GetTopRankPlayer(CancellationToken cancellationToken)
        {
            var topPlayer = await _dataContext.AccountStatsSummaries.OrderByDescending(x => x.Score).Take(20)
                .ToListAsync(cancellationToken);

            var player = topPlayer.Select(p =>
               AccountStatsSummaryViewModel.Map(p, _dataContext.Accounts.FirstOrDefault(x => x.Id == p.AccountId))).ToList();

            return OkData(player);

        }

        private async Task<int> CalculatePlayerSkyAsync(long accountId, CancellationToken cancellationToken)
        {
            var battleCorrectAnswersCount = await _dataContext.OneToOneBattles
                .Where(q => q.Player1Id == accountId || q.Player2Id == accountId)
                .GroupBy(q => q.Player1Id == accountId ? q.Player1CorrectAnswersCount : q.Player2CorrectAnswersCount)
                .Select(q => new { CorrectAnswersCount = q.Key, BattlesCount = q.Count() }).ToListAsync(cancellationToken);

            var totalBattlesCount = battleCorrectAnswersCount.Sum(q => q.BattlesCount);

            int sky;

            double? averageAnswersCountIn70PercentOfBattles = Math.Round(
                battleCorrectAnswersCount.OrderByDescending(q => q.CorrectAnswersCount).Take((int)(totalBattlesCount * 0.7f))
                    .Average(q => (double?)q.CorrectAnswersCount) ?? 0, 0);

            if (averageAnswersCountIn70PercentOfBattles >= 14)
                sky = 7;
            else if (averageAnswersCountIn70PercentOfBattles >= 13)
                sky = 6;
            else if (averageAnswersCountIn70PercentOfBattles >= 12)
                sky = 5;
            else if (averageAnswersCountIn70PercentOfBattles >= 11)
                sky = 4;
            else if (averageAnswersCountIn70PercentOfBattles >= 10)
                sky = 3;
            else if (averageAnswersCountIn70PercentOfBattles >= 9)
                sky = 2;
            else if (averageAnswersCountIn70PercentOfBattles >= 8)
                sky = 1;
            else
                sky = 0;
            return sky;
        }

        private async Task<IActionResult> ChangeBattleInvitationStatus(long battleInvitationId, CancellationToken cancellationToken)
        {
            var invitation = await _dataContext.BattleInvitations.FirstOrDefaultAsync(q => q.Id == battleInvitationId, cancellationToken);

            if (invitation == null)
                return NotFound();

            if (invitation.Status != BattleInvitationStatus.Pending)
                return BadRequest("invalid_state", "Invitation must be in pending state.");

            invitation.Status = BattleInvitationStatus.Rejected;
            await _dataContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}