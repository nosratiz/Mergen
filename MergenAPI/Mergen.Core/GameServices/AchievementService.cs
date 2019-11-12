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
    public class AchievementService
    {
        private readonly DataContext _context;

        public AchievementService(DataContext context)
        {
            _context = context;
        }

        public async Task ProcessBattleAchievementsAsync(OneToOneBattle battle, AccountStatsSummary player1Stats, AccountStatsSummary player2Stats, CancellationToken cancellationToken)
        {
            var achievementTypes = await _context.AchievementTypes.AsNoTracking().Where(q => q.IsArchived == false).ToListAsync(cancellationToken);

            var achievements = await _context.Achievements.Where(q => q.IsArchived == false && (q.AccountId == battle.Player1Id || q.AccountId == battle.Player2Id)).ToListAsync(cancellationToken);
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

            await _context.SaveChangesAsync(cancellationToken);
        }

        private void ProcessWinnerAchievement(long playerId, AccountStatsSummary playerStats,
            Dictionary<long, Achievement> playerAchievements, AchievementType a)
        {
            if (!playerAchievements.ContainsKey(a.Id) && playerStats.WinCount >= a.WinnedBattlesCount)
            {
                _context.Achievements.Add(new Achievement
                {
                    AccountId = playerId,
                    AchievementTypeId = a.Id,
                    AchieveDateTime = DateTime.UtcNow
                });
            }
        }

        private void ProcessAceWinAchievement(long playerId, AccountStatsSummary playerStats,
            Dictionary<long, Achievement> playerAchievements, AchievementType a)
        {
            if (!playerAchievements.ContainsKey(a.Id) && playerStats.AceWinCount >= a.AceWinCount)
            {
                _context.Achievements.Add(new Achievement
                {
                    AccountId = playerId,
                    AchievementTypeId = a.Id,
                    AchieveDateTime = DateTime.UtcNow
                });
            }
        }

        private void ProcessTotalBattlesAchievement(long playerId, AccountStatsSummary playerStats,
            Dictionary<long, Achievement> playerAchievements, AchievementType a)
        {
            if (!playerAchievements.ContainsKey(a.Id) && playerStats.TotalBattlesPlayed >= a.NumberOfTotalBattlesPlayed)
            {
                _context.Achievements.Add(new Achievement
                {
                    AccountId = playerId,
                    AchievementTypeId = a.Id,
                    AchieveDateTime = DateTime.UtcNow
                });
            }
        }

        public async Task ProcessSuccessfulBattleInvitationAchievements(BattleInvitation battleInvitation,
            AccountStatsSummary inviterPlayerStats, CancellationToken cancellationToken)
        {
            var achievementTypes = await _context.AchievementTypes.AsNoTracking().Where(q => q.IsArchived == false && q.NumberOfSuccessfulBattleInvitations != null).ToListAsync(cancellationToken);

            foreach (var a in achievementTypes)
            {
                if (inviterPlayerStats.SuccessfulBattleInvitationsCount >= a.NumberOfSuccessfulBattleInvitations)
                {
                    _context.Achievements.Add(new Achievement
                    {
                        AccountId = battleInvitation.AccountId,
                        AchievementTypeId = a.Id,
                        AchieveDateTime = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ProcessAnswerTimeAchievementsAsync(AccountStatsSummary playerStat, List<AccountCategoryStat> changedCategoryStats, CancellationToken cancellationToken)
        {
            var achievementTypes = await _context.AchievementTypes.AsNoTracking().Where(q => q.IsArchived == false).ToListAsync(cancellationToken);

            foreach (var a in achievementTypes)
            {
                if (a.CorrectAnswersCountInCategory != null)
                {
                    var catStat = changedCategoryStats.FirstOrDefault(q => q.CategoryId == a.CategoryId);
                    if (catStat != null)
                    {
                        if (catStat.CorrectAnswersCount >= a.CorrectAnswersCountInCategory)
                        {
                            _context.Achievements.Add(new Achievement
                            {
                                AccountId = playerStat.AccountId,
                                AchievementTypeId = a.Id,
                                AchieveDateTime = DateTime.UtcNow,
                                TitleAchievement = $"Category {catStat.Category.Title}"
                            });
                        }
                    }
                }
                if (a.RemoveTwoAnswersHelperUsageCount != null)
                {
                    if (playerStat.RemoveTwoAnswersHelperUsageCount >= a.RemoveTwoAnswersHelperUsageCount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Facilitates Remove Two Answers {a.Title}"
                        });
                    }
                }
                if (a.AnswerHistoryHelperUsageCount != null)
                {
                    if (playerStat.AnswerHistoryHelperUsageCount >= a.AnswerHistoryHelperUsageCount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Facilitates Answer History {a.Title}"
                        });
                    }
                }
                if (a.AskMergenHelperUsageCount != null)
                {
                    if (playerStat.AskMergenHelperUsageCount >= a.AskMergenHelperUsageCount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Facilitates Ask Mergen Helper {a.Title}"
                        });
                    }
                }
                if (a.DoubleChanceHelperUsageCount != null)
                {
                    if (playerStat.DoubleChanceHelperUsageCount >= a.DoubleChanceHelperUsageCount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Facilitates Double Chance Helper {a.Title}"
                        });
                    }
                }

                if (a.NumberOfContinuousDaysPlaying != null)
                {
                    //for (int i = 0; i < 30; i++)
                    //{
                    //    var continuous = await _context.OneToOneBattles.AnyAsync(x =>
                    //        x.IsArchived == false && x.StartDateTime == DateTime.Today.AddDays(-i), cancellationToken);

                    //    if (continuous)
                    //    {
                    //        playerStat.ContinuousActiveDaysCount += 1;
                    //        _context.AccountStatsSummaries.Update(playerStat);
                    //        await _context.SaveChangesAsync(cancellationToken);
                    //    }
                    //    else
                    //        break;


                    //}

                    if (a.NumberOfContinuousDaysPlaying == playerStat.ContinuousActiveDaysCount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Continuous days play {a.Title}"
                        });
                    }


                }

                if (a.GiftedCoinsAmount != null)
                {
                    if (playerStat.GiftedCoins == a.GiftedCoinsAmount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Generous {a.Title}"
                        });
                    }
                }

                if (a.NumberOfTotalBattlesPlayed != null)
                {
                    if (a.NumberOfTotalBattlesPlayed == playerStat.TotalBattlesPlayed)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Total Games {a.Title}"
                        });
                    }
                }

                if (a.NumberOfSuccessfulBattleInvitations != null)
                {
                    if (a.NumberOfSuccessfulBattleInvitations == playerStat.InvitedPlayersCount)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Invite {a.Title}"
                        });
                    }
                }

                if (a.CoinsSpentOnAvatarItems != null)
                {
                    if (a.CoinsSpentOnAvatarItems == playerStat.CoinsSpentOnAvatarItems)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Avatar {a.Title}"
                        });
                    }
                }

                if (a.CoinsSpentOnBooster != null)
                {
                    if (a.CoinsSpentOnBooster == playerStat.CoinsSpentOnBoosterItems)
                    {
                        _context.Achievements.Add(new Achievement
                        {
                            AccountId = playerStat.AccountId,
                            AchievementTypeId = a.Id,
                            AchieveDateTime = DateTime.UtcNow,
                            TitleAchievement = $"Boost {a.Title}"
                        });
                    }

                }


            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ProcessPaymentAchievementsAsync(AccountStatsSummary playerStats, Payment payment, CancellationToken cancellationToken)
        {
            var achievementTypes = await _context.AchievementTypes.Where(q => q.IsArchived == false && (q.CoinsSpentOnAvatarItems != null || q.CoinsSpentOnBooster != null)).ToListAsync(cancellationToken);

            foreach (var a in achievementTypes)
            {
                if (a.CoinsSpentOnAvatarItems != null && payment.ShopItem.TypeId == ShopItemTypeIds.AvatarItem && playerStats.CoinsSpentOnAvatarItems >= a.CoinsSpentOnAvatarItems)
                {
                    _context.Achievements.Add(new Achievement
                    {
                        AccountId = playerStats.AccountId,
                        AchievementTypeId = a.Id,
                        AchieveDateTime = DateTime.UtcNow
                    });
                }
                else if (a.CoinsSpentOnBooster != null && payment.ShopItem.TypeId == ShopItemTypeIds.Booster && playerStats.CoinsSpentOnBoosterItems >= a.CoinsSpentOnBooster)
                {
                    _context.Achievements.Add(new Achievement
                    {
                        AccountId = playerStats.AccountId,
                        AchievementTypeId = a.Id,
                        AchieveDateTime = DateTime.UtcNow
                    });
                }
            }
        }


    }
}