using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.GameServices
{
    public class GamingService
    {
        private readonly DataContext _dataContext;

        public GamingService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<OneToOneBattle> StartRandomBattleAsync(Account player1, Account player2 = default,
            CancellationToken cancellationToken = default)
        {
            OneToOneBattle battle;

            var pendingBattle = await _dataContext.OneToOneBattles.OrderBy(q => q.StartDateTime)
                .Include(q => q.LastGame)
                .FirstOrDefaultAsync(q => q.Player1Id != player1.Id && q.Player2Id == null, cancellationToken);

            if (pendingBattle != null)
            {
                pendingBattle.Player2Id = player1.Id;
                battle = pendingBattle;

                if (pendingBattle.LastGame != null && pendingBattle.LastGame.GameState == GameStateIds.Player2AnswerQuestions)
                {
                    pendingBattle.LastGame.CurrentTurnPlayerId = player1.Id;
                }
            }
            else
            {
                battle = await StartNewRandomBattleAsync(player1, player2, cancellationToken);
            }

            await _dataContext.SaveChangesAsync(cancellationToken);
            return battle;
        }

        private async Task<OneToOneBattle> StartNewRandomBattleAsync(Account player1, Account player2 = default,
            CancellationToken cancellationToken = default)
        {
            var battle = new OneToOneBattle
            {
                BattleType = BattleType.OneOnOne,
                Player1Id = player1.Id,
                Player2Id = player2?.Id,
                Round = 1,
                StartDateTime = DateTime.UtcNow,
                BattleStateId = BattleStateIds.SelectCategory
            };

            _dataContext.Battles.Add(battle);

            var startingGame = await CreateGameAsync(battle, player1, cancellationToken);
            battle.Games.Add(startingGame);
            startingGame.Battle = battle;
            await _dataContext.SaveChangesAsync(cancellationToken);

            //battle.LastGame = startingGame;
            battle.LastGameId = startingGame.Id;

            return battle;
        }

        public async Task<Game> CreateGameAsync(OneToOneBattle battle, Account player, CancellationToken cancellationToken = default)
        {
            var game = new Game
            {
                CurrentTurnPlayerId = player?.Id,
                GameState = GameStateIds.SelectCategory,
                Battle = battle
            };

            var randomCategoryIds = new HashSet<long>();
            while (game.GameCategories.Count < 3)
            {
                var randomCategory = await _dataContext.Categories.Where(q => q.IsArchived == false).OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync(cancellationToken);
                if (randomCategory != null && randomCategoryIds.Add(randomCategory.Id))
                {
                    game.GameCategories.Add(new GameCategory
                    {
                        CategoryId = randomCategory.Id
                    });
                }
            }

            _dataContext.Games.Add(game);
            return game;
        }

        public async Task<Result> SelectCategoryAsync(long battleId, long playerId, long categoryId,
            CancellationToken cancellationToken = default)
        {
            var battle = await _dataContext.OneToOneBattles.Include(q => q.LastGame).ThenInclude(q => q.GameCategories)
                .FirstAsync(q => q.Id == battleId, cancellationToken: cancellationToken);

            if (battle.LastGame.CurrentTurnPlayerId != playerId)
                return Result.Error(StatusCode.Forbidden);

            if (battle.LastGame.GameCategories.All(q => q.CategoryId != categoryId))
                return Result.Error(StatusCode.Forbidden);

            battle.LastGame.SelectedCategoryId = categoryId;
            battle.LastGame.GameState = playerId == battle.Player1Id ? GameStateIds.Player1AnswerQuestions : GameStateIds.Player2AnswerQuestions;

            // add random questions to battle
            var questions = await _dataContext.QuestionCategories.Where(q => q.CategoryId == categoryId).OrderBy(r => Guid.NewGuid()).Take(3).ToListAsync(cancellationToken);
            var gameQuestions = questions.Select(q => new GameQuestion
            {
                GameId = battle.LastGameId.Value,
                QuestionId = q.QuestionId
            });
            battle.LastGame.GameQuestions = gameQuestions.ToList();

            battle.BattleStateId = BattleStateIds.AnsweringQuestions;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RandomizeCategories(Game game, CancellationToken cancellationToken)
        {
            var oldCategories = game.GameCategories.Select(q => q.CategoryId).ToList();
            game.GameCategories.Clear();

            var randomCategoryIds = new HashSet<long>();
            while (game.GameCategories.Count < 3)
            {
                var randomCategory = await _dataContext.Categories.Where(q => q.IsArchived == false).OrderBy(q => Guid.NewGuid())
                    .FirstOrDefaultAsync(cancellationToken);
                if (randomCategory != null && !oldCategories.Contains(randomCategory.Id) && randomCategoryIds.Add(randomCategory.Id))
                {
                    game.GameCategories.Add(new GameCategory
                    {
                        CategoryId = randomCategory.Id
                    });
                }
            }

            return Result.Success();
        }
    }
}