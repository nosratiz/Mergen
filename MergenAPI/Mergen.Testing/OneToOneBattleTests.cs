using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mergen.Core;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mergen.Testing
{
    public class OneToOneBattleTests
    {
        private GamingService _gamingService;
        private Account _player1;
        private Account _player2;

        public OneToOneBattleTests()
        {
            IServiceCollection services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            services.RegisterMergenServices(configuration, true);
            var sp = services.BuildServiceProvider();
            ApplicationEvents.ApplicationStart(sp, configuration);

            var db = sp.GetRequiredService<DataContext>();
            _gamingService = new GamingService(db);

            _player1 = new Account
            {
                Id = 1
            };

            _player2 = new Account
            {
                Id = 2
            };

            for (int i = 0; i < 100; i++)
            {
                db.Categories.Add(new Category { Title = "Cat " + i });
                db.Questions.Add(new Question { Body = "Body " + i, Answer1 = "Answer1 " + i, Answer2 = "Answer2 " + i });
            }

            db.SaveChanges();
        }

        [Fact]
        public async Task RandomBattle()
        {
            var battle = await _gamingService.StartRandomBattleAsync(_player1);
            var selectedCategoryId = battle.LastGame.GameCategories.OrderBy(q => Guid.NewGuid()).First().CategoryId;
            await _gamingService.SelectCategoryAsync(battle.Id, _player1.Id, selectedCategoryId);
            Assert.Equal(selectedCategoryId, battle.LastGame.SelectedCategoryId);
        }
    }
}