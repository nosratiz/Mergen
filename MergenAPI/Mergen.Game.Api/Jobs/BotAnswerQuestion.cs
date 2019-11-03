using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mergen.Game.Api.Jobs
{
    public class BotAnswerQuestion : IJob
    {
        private const int ExperienceBase = 10;
        private const int WinExperienceMultiplier = 3;
        private const int LoseExperienceMultiplier = 1;
        private const int DrawExperienceMultiplier = 2;

        private const int CoinBase = 10;
        private const int WinCoinMultiplier = 2;
        private const int LoseCoinMultiplier = 1;
        private readonly ConnectionStringOption _connectionStringOption;
        private readonly LevelManager _levelManager;

        public BotAnswerQuestion(IOptions<ConnectionStringOption> connectionStringOption, LevelManager levelManager)
        {
            _levelManager = levelManager;
            _connectionStringOption = connectionStringOption.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);

            using (DataContext dataContext = new DataContext(option.Options))
            {
                var games = await dataContext.Games.Where(x => x.IsArchived == false && x.GameState == GameStateIds.Player2AnswerQuestions).ToListAsync();

                foreach (var game in games)
                {
                    var isBotPlayer = await dataContext.Accounts.FirstOrDefaultAsync(x => x.IsBot && x.Id == game.CurrentTurnPlayerId);

                    if (isBotPlayer is null)
                        continue;

                    var playerStat = await dataContext.AccountStatsSummaries.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId);
                    if (playerStat == null)
                    {
                        playerStat = new AccountStatsSummary
                        {
                            AccountId = game.CurrentTurnPlayerId.Value
                        };

                        dataContext.AccountStatsSummaries.Add(playerStat);
                    }

                    var gameQuestions = await dataContext.GameQuestions.Include(x => x.Question)
                        .ThenInclude(x => x.QuestionCategories).Where(x => x.GameId == game.Id).ToListAsync();

                    if (gameQuestions.Count == 0)
                        continue;

                    var changedCategoryStats = new List<AccountCategoryStat>();

                    foreach (var gameQuestion in gameQuestions)
                    {
                        var randomAnswer = new Random().Next(1, 4);

                        gameQuestion.Player2SelectedAnswer = randomAnswer;

                        var score = gameQuestion.Question.CorrectAnswerNumber == randomAnswer ? 1 : 0;

                        switch (randomAnswer)
                        {
                            case 1:
                                gameQuestion.Question.Answer1ChooseHistory++;
                                break;

                            case 2:
                                gameQuestion.Question.Answer2ChooseHistory++;
                                break;

                            case 3:
                                gameQuestion.Question.Answer3ChooseHistory++;
                                break;

                            case 4:
                                gameQuestion.Question.Answer4ChooseHistory++;
                                break;
                        }

                        foreach (var category in gameQuestion.Question.QuestionCategories)
                        {
                            var playerCategoryStat = await dataContext.AccountCategoryStats.FirstOrDefaultAsync(q => q.AccountId == game.CurrentTurnPlayerId && q.CategoryId == category.Id);
                            if (playerCategoryStat == null)
                            {
                                playerCategoryStat = new AccountCategoryStat
                                {
                                    AccountId = game.CurrentTurnPlayerId.Value,
                                    CategoryId = category.CategoryId
                                };

                                dataContext.AccountCategoryStats.Add(playerCategoryStat);
                                changedCategoryStats.Add(playerCategoryStat);
                            }

                            playerCategoryStat.TotalQuestionsCount += 1;
                            playerCategoryStat.CorrectAnswersCount += score;
                        }
                    }

                    var achievementTypess = await dataContext.AchievementTypes.AsNoTracking().Where(q => q.IsArchived == false).ToListAsync();

                    foreach (var a in achievementTypess)
                    {
                        if (a.CorrectAnswersCountInCategory != null)
                        {
                            var catStat = changedCategoryStats.FirstOrDefault(q => q.CategoryId == a.CategoryId);
                            if (catStat != null)
                            {
                                if (catStat.CorrectAnswersCount >= a.CorrectAnswersCountInCategory)
                                {
                                    dataContext.Achievements.Add(new Achievement
                                    {
                                        AccountId = playerStat.AccountId,
                                        AchievementTypeId = a.Id,
                                        AchieveDateTime = DateTime.UtcNow
                                    });
                                }
                            }
                        }
                    }

                    var gameBattleId = game.BattleId;
                    if (game.GameQuestions.All(q => q.Player1SelectedAnswer.HasValue && q.Player2SelectedAnswer.HasValue))
                    {
                        game.GameState = GameStateIds.Completed;

                        var battle = await dataContext.OneToOneBattles
                            .Include(q => q.Games).ThenInclude(q => q.GameQuestions).ThenInclude(q => q.Question)
                            .Include(q => q.Player1)
                            .Include(q => q.Player2)
                            .FirstOrDefaultAsync(q => q.Id == gameBattleId);

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

                            var playersStats = await dataContext.AccountStatsSummaries
                                .Where(q => q.AccountId == battle.Player1Id || q.AccountId == battle.Player2Id)
                                .ToDictionaryAsync(q => q.AccountId, q => q);

                            var player1Stats = playersStats[battle.Player1Id];
                            var player2Stats = playersStats[battle.Player2Id.Value];

                            player1Stats.TotalBattlesPlayed += 1;
                            player2Stats.TotalBattlesPlayed += 1;

                            if (battle.WinnerPlayerId == battle.Player1Id)
                            {
                                player1Stats.WinCount += 1;
                                player2Stats.LoseCount += 1;

                                if (player1CorrectAnswersCount == 15)
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

                                if (player2CorrectAnswersCount == 15)
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

                            var achievementTypes = await dataContext.AchievementTypes.AsNoTracking().Where(q => q.IsArchived == false).ToListAsync();

                            var achievements = await dataContext.Achievements.Where(q => q.IsArchived == false && (q.AccountId == battle.Player1Id || q.AccountId == battle.Player2Id)).ToListAsync();
                            var player1Achievements = achievements.Where(q => q.AccountId == battle.Player1Id)
                                .ToDictionary(q => q.AchievementTypeId);

                            var player2Achievements = achievements.Where(q => q.AccountId == battle.Player1Id)
                                .ToDictionary(q => q.AchievementTypeId);

                            // Winner
                            foreach (var a in achievementTypes)
                            {
                                if (a.WinnedBattlesCount != null)
                                {
                                    ProcessWinnerAchievement(battle.Player1Id, player1Stats, player1Achievements, a);
                                    ProcessWinnerAchievement(battle.Player2Id.Value, player2Stats, player2Achievements, a);
                                }
                                else if (a.AceWinCount != null)
                                {
                                    ProcessAceWinAchievement(battle.Player1Id, player1Stats, player1Achievements, a);
                                    ProcessAceWinAchievement(battle.Player2Id.Value, player2Stats, player2Achievements, a);
                                }
                                else if (a.NumberOfTotalBattlesPlayed != null)
                                {
                                    ProcessTotalBattlesAchievement(battle.Player1Id, player1Stats, player1Achievements, a);
                                    ProcessTotalBattlesAchievement(battle.Player2Id.Value, player2Stats, player2Achievements, a);
                                }
                            }

                            await dataContext.SaveChangesAsync();

                            await dataContext.SaveChangesAsync();

                            // Calculate Sky
                            player1Stats.Sky = await CalculatePlayerSkyAsync(battle.Player1Id);
                            player2Stats.Sky = await CalculatePlayerSkyAsync(battle.Player2Id.Value);
                        }
                        else
                        {
                            var nextPlayer = game.CurrentTurnPlayerId == battle.Player1Id ? battle.Player1 : battle.Player2;
                            var newGame = new Core.Entities.Game
                            {
                                CurrentTurnPlayerId = nextPlayer.Id,
                                GameState = GameStateIds.SelectCategory,
                                Battle = battle
                            };

                            var randomCategoryIds = new HashSet<long>();
                            while (newGame.GameCategories.Count < 3)
                            {
                                var randomCategory = await dataContext.Categories.Where(q => q.IsArchived == false).OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();
                                if (randomCategory != null && randomCategoryIds.Add(randomCategory.Id))
                                {
                                    newGame.GameCategories.Add(new GameCategory
                                    {
                                        CategoryId = randomCategory.Id
                                    });
                                }
                            }

                            battle.Games.Add(newGame);
                            dataContext.Games.Add(newGame);
                            battle.LastGame = newGame;
                        }
                    }
                    else
                    {
                        var battle = await dataContext.OneToOneBattles
                            .Include(q => q.Player1)
                            .Include(q => q.Player2)
                            .FirstOrDefaultAsync(q => q.Id == gameBattleId);

                        var nextPlayer = game.CurrentTurnPlayerId == battle.Player1Id ? battle.Player2 : battle.Player1;
                        game.CurrentTurnPlayerId = nextPlayer?.Id;
                        game.GameState = game.CurrentTurnPlayerId == battle.Player1Id ? GameStateIds.Player1AnswerQuestions : GameStateIds.Player2AnswerQuestions;
                    }
                    await dataContext.SaveChangesAsync();
                }
            }
        }

        private async Task<int> CalculatePlayerSkyAsync(long accountId)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);

            using (DataContext dataContext = new DataContext(option.Options))
            {
                var battleCorrectAnswersCount = await dataContext.OneToOneBattles
                    .Where(q => q.Player1Id == accountId || q.Player2Id == accountId)
                    .GroupBy(
                        q => q.Player1Id == accountId ? q.Player1CorrectAnswersCount : q.Player2CorrectAnswersCount)
                    .Select(q => new { CorrectAnswersCount = q.Key, BattlesCount = q.Count() })
                    .ToListAsync();

                var totalBattlesCount = battleCorrectAnswersCount.Sum(q => q.BattlesCount);

                int sky;

                double? averageAnswersCountIn70PercentOfBattles = Math.Round(
                    battleCorrectAnswersCount.OrderByDescending(q => q.CorrectAnswersCount)
                        .Take((int)(totalBattlesCount * 0.7f))
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
        }

        private void ProcessWinnerAchievement(long playerId, AccountStatsSummary playerStats,
            Dictionary<long, Achievement> playerAchievements, AchievementType a)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);

            using (DataContext dataContext = new DataContext(option.Options))
            {
                if (!playerAchievements.ContainsKey(a.Id) && playerStats.WinCount >= a.WinnedBattlesCount)
                {
                    dataContext.Achievements.Add(new Achievement
                    {
                        AccountId = playerId,
                        AchievementTypeId = a.Id,
                        AchieveDateTime = DateTime.UtcNow
                    });
                }
            }
        }

        private void ProcessAceWinAchievement(long playerId, AccountStatsSummary playerStats,
            Dictionary<long, Achievement> playerAchievements, AchievementType a)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);
            using (DataContext dataContext = new DataContext(option.Options))
            {
                if (!playerAchievements.ContainsKey(a.Id) && playerStats.AceWinCount >= a.AceWinCount)
                {
                    dataContext.Achievements.Add(new Achievement
                    {
                        AccountId = playerId,
                        AchievementTypeId = a.Id,
                        AchieveDateTime = DateTime.UtcNow
                    });
                }
            }
        }

        private void ProcessTotalBattlesAchievement(long playerId, AccountStatsSummary playerStats,
            Dictionary<long, Achievement> playerAchievements, AchievementType a)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);
            using (DataContext dataContext = new DataContext(option.Options))
            {
                if (!playerAchievements.ContainsKey(a.Id) &&
                    playerStats.TotalBattlesPlayed >= a.NumberOfTotalBattlesPlayed)
                {
                    dataContext.Achievements.Add(new Achievement
                    {
                        AccountId = playerId,
                        AchievementTypeId = a.Id,
                        AchieveDateTime = DateTime.UtcNow
                    });
                }
            }
        }
    }
}