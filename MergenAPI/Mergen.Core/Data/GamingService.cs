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

        public async Task<OneToOneBattle> StartRandomBattleAsync(Account player1,
            CancellationToken cancellationToken = default)
        {
            OneToOneBattle battle;

            var pendingBattle = await _dataContext.OneToOneBattles.OrderBy(q => q.StartDateTime)
                .FirstOrDefaultAsync(q => q.Player2Id == null, cancellationToken);

            if (pendingBattle != null)
            {
                pendingBattle.Player2Id = player1.Id;
                battle = pendingBattle;
            }
            else
            {
                battle = new OneToOneBattle
                {
                    BattleType = BattleType.OneOnOne,
                    Player1Id = player1.Id,
                    StartDateTime = DateTime.UtcNow,
                    Round = 1,
                    BattleState = BattleState.SelectCategory
                };

                var startingGame = await CreateGameAsync(player1, cancellationToken);
                battle.Games.Add(startingGame);

                battle.LastGame = startingGame;

                _dataContext.OneToOneBattles.Add(battle);
            }

            await _dataContext.SaveChangesAsync(cancellationToken);
            return battle;
        }

        public async Task<OneToOneBattle> StartBattleWithPlayerAsync(Account player1, Account player2,
            CancellationToken cancellationToken = default)
        {
            var battle = new OneToOneBattle
            {
                BattleType = BattleType.OneOnOne,
                Player1Id = player1.Id,
                Player2Id = player2.Id,
                Round = 1,
                StartDateTime = DateTime.UtcNow,
                BattleState = BattleState.SelectCategory
            };

            var startingGame = await CreateGameAsync(player1, cancellationToken);
            battle.Games.Add(startingGame);

            battle.LastGame = startingGame;

            await _dataContext.SaveChangesAsync(cancellationToken);
            return battle;
        }

        public async Task<Game> CreateGameAsync(Account player, CancellationToken cancellationToken = default)
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

            battle.BattleState = BattleState.AnsweringQuestions;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

    public enum StatusCode
    {
        Success,
        Forbidden
    }

    public class Result
    {
        public Result(StatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public bool IsSuccess => StatusCode == StatusCode.Success;
        public StatusCode StatusCode { get; protected set; }

        public static Result Error(StatusCode statusCode)
        {
            return new Result(statusCode);
        }

        public static Result<T> Error<T>(StatusCode statusCode)
        {
            return new Result<T>(statusCode, default);
        }

        public static Result Success()
        {
            return new Result(StatusCode.Success);
        }

        public static Result<T> Success<T>(T value)
        {
            return new Result<T>(StatusCode.Success, value);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        public Result(StatusCode statusCode, T value) : base(statusCode)
        {
            Value = value;
        }
    }
}