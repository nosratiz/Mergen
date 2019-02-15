using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Data
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
                .FirstOrDefaultAsync(q => q.Player1Id != player1.Id && q.Player2Id == null, cancellationToken);

            if (pendingBattle != null)
            {
                pendingBattle.Player2Id = player1.Id;
                battle = pendingBattle;
            }
            else
            {
                battle = await StartNewRandomBattleAsync(player1, player2, cancellationToken);
                _dataContext.OneToOneBattles.Add(battle);
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

            var startingGame = await CreateGameAsync(player1, cancellationToken);
            battle.Games.Add(startingGame);

            battle.LastGame = startingGame;

            await _dataContext.SaveChangesAsync(cancellationToken);
            return battle;
        }

        private async Task<Game> CreateGameAsync(Account player, CancellationToken cancellationToken = default)
        {
            var game = new Game
            {
                PlayerId = player.Id,
                GameState = GameState.SelectCategory
            };

            var randomCategoryIds = new HashSet<long>();
            while (game.GameCategories.Count < 3)
            {
                var randomCategory = await _dataContext.Categories.OrderBy(q => Guid.NewGuid())
                    .FirstOrDefaultAsync(cancellationToken);
                if (randomCategory != null && randomCategoryIds.Add(randomCategory.Id))
                {
                    game.GameCategories.Add(new GameCategory
                    {
                        CategoryId = randomCategory.Id
                    });
                }
            }

            return game;
        }

        public async Task<Result> SelectCategoryAsync(long battleId, long playerId, long categoryId,
            CancellationToken cancellationToken = default)
        {
            var battle = await _dataContext.OneToOneBattles.Include(q => q.LastGame).ThenInclude(q => q.GameCategories)
                .FirstAsync(q => q.Id == battleId, cancellationToken: cancellationToken);

            if (battle.LastGame.PlayerId != playerId)
                return Result.Error(StatusCode.Forbidden);

            if (battle.LastGame.GameCategories.All(q => q.CategoryId != categoryId))
                return Result.Error(StatusCode.Forbidden);

            battle.LastGame.SelectedCategoryId = categoryId;
            battle.LastGame.GameState = GameState.AnswerQuestions;

            // TODO: add random questions to battle

            battle.BattleStateId = BattleStateIds.AnsweringQuestions;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}