using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace Mergen.Game.Api.Jobs
{
    public class FindPlayer : IJob
    {

        private readonly BattleManager _battleManager;
        private readonly ConnectionStringOption _connectionStringOption;


        public FindPlayer(BattleManager battleManager, IOptions<ConnectionStringOption> connectionStringOption)
        {

            _battleManager = battleManager;
            _connectionStringOption = connectionStringOption.Value;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);

            using (DataContext dataContext = new DataContext(option.Options))
            {
                var battles = await dataContext.OneToOneBattles
                    .Where(x => x.BattleStateId == BattleStateIds.WaitingForOpponent && x.CreationDateTime <= DateTime.Now.AddMinutes(2) && x.IsArchived == false).ToListAsync();

                var botPlayers = await dataContext.Accounts.Where(x => x.IsBot).ToListAsync();

                foreach (var battle in battles)
                {
                    var player = new Random().Next(botPlayers.Count);
                    battle.Player2Id = botPlayers[player].Id;
                    battle.StartDateTime = DateTime.Now;
                    battle.BattleStateId = BattleStateIds.SelectCategory;
                    await _battleManager.SaveAsync(battle);

                    var game = new Core.Entities.Game
                    {
                        CurrentTurnPlayerId = battle.Player2Id,
                        GameState = GameStateIds.SelectCategory,
                        Battle = battle
                    };

                    var randomCategoryIds = new HashSet<long>();
                    while (game.GameCategories.Count < 3)
                    {
                        var randomCategory = await dataContext.Categories.Where(q => q.IsArchived == false).OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();
                        if (randomCategory != null && randomCategoryIds.Add(randomCategory.Id))
                        {
                            game.GameCategories.Add(new GameCategory
                            {
                                CategoryId = randomCategory.Id
                            });
                        }
                    }

                    battle.Games.Add(game);
                    dataContext.Games.Add(game);

                    battle.Games.Add(game);
                    game.Battle = battle;
                    battle.LastGameId = game.Id;
                    await dataContext.SaveChangesAsync();

                   
                }
            }

        }

    }
}