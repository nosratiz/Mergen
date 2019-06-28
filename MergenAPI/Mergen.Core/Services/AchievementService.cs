using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Services
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
    }
}