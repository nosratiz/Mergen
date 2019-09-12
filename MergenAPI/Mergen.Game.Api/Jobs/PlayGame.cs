using System;
using System.Linq;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace Mergen.Game.Api.Jobs
{
    public class PlayGame : IJob
    {
        private readonly ConnectionStringOption _connectionStringOption;

        public PlayGame(IOptions<ConnectionStringOption> connectionStringOption)
        {
            _connectionStringOption = connectionStringOption.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);

            using (DataContext dataContext = new DataContext(option.Options))
            {
                var games = await dataContext.Games.Include(x => x.Battle)
                    .Where(x => (x.SelectedCategoryId == null) && x.IsArchived == false)
                    .ToListAsync();

                foreach (var game in games)
                {
                    var isBotPlayer = await dataContext.Accounts.FirstOrDefaultAsync(x => x.IsBot && x.Id == game.CurrentTurnPlayerId);

                    if (isBotPlayer is null)
                        continue;

                    var gameCategories = await dataContext.GameCategories.Where(x => x.GameId == game.Id).ToListAsync();
                    var categoryId = new Random().Next(1,3);



                    game.GameState = GameStateIds.Player2AnswerQuestions;
                    game.SelectedCategoryId = categoryId;
                    var battle = (OneToOneBattle)game.Battle;
                    battle.BattleStateId = BattleStateIds.AnsweringQuestions;


                    // add random questions to battle
                    var questions = await dataContext.QuestionCategories.Include(q => q.Question)
                        .Where(q => q.CategoryId == categoryId).OrderBy(r => Guid.NewGuid()).Take(3).ToListAsync();
                    var gameQuestions = questions.Select(q => new GameQuestion
                    {
                        GameId = game.Id,
                        QuestionId = q.QuestionId,
                        Question = q.Question
                    });
                    game.GameQuestions = gameQuestions.ToList();

                    //dataContext.GameQuestions.AddRange(gameQuestions);

                    await dataContext.SaveChangesAsync();
                }
            }
        }
    }
}