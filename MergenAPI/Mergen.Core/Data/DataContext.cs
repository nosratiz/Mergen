using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Entities;
using Mergen.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EfHelpers.DisableCascadeDeletes(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameQuestion>().HasKey(e => new { e.GameId, e.QuestionId });
            modelBuilder.Entity<GameCategory>().HasKey(e => new { e.GameId, e.CategoryId });
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountInvitation> AccountInvitations { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<AccountStatsSummary> AccountStatsSummaries { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<AchievementType> AchievementTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionCategory> QuestionCategories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<PurchaseLog> PurchaseLogs { get; set; }
        public DbSet<AccountItem> AccountItems { get; set; }

        public DbSet<Battle> Battles { get; set; }
        public DbSet<OneToOneBattle> OneToOneBattles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameCategory> GameCategories { get; set; }
        public DbSet<GameQuestion> GameQuestions { get; set; }

    }

    public class Battle : Entity
    {
        public BattleType BattleType { get; set; }
        public DateTime StartDateTime { get; set; }
    }

    public class OneToOneBattle : Battle
    {
        public OneToOneBattle()
        {
            Games = new Collection<Game>();
        }

        public long? Player1Id { get; set; }
        public Account Player1 { get; set; }

        public long? Player2Id { get; set; }
        public Account Player2 { get; set; }
        public int Round { get; set; }
        public long LastGameId { get; set; }
        public Game LastGame { get; set; }

        public ICollection<Game> Games { get; set; }

        public BattleState BattleState { get; set; }
    }

    public enum BattleType
    {
        OneOnOne
    }

    public enum BattleState
    {
        SelectCategory,
        AnsweringQuestions,
        Completed
    }

    public enum GameState
    {
        SelectCategory,
        AnswerQuestions,
        Completed
    }

    public class Game : Entity
    {
        public Game()
        {
            GameCategories = new Collection<GameCategory>();
            GameQuestions = new Collection<GameQuestion>();
        }

        public long PlayerId { get; set; }
        public Account Player { get; set; }

        public long SelectedCategoryId { get; set; }
        public Category SelectedCategory { get; set; }

        public ICollection<GameCategory> GameCategories { get; set; }
        public ICollection<GameQuestion> GameQuestions { get; set; }

        public GameState GameState { get; set; }
    }

    public class GameCategory
    {
        public long GameId { get; set; }
        public Game Game { get; set; }

        public long CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public class GameQuestion
    {
        public long GameId { get; set; }
        public Game Game { get; set; }

        public long QuestionId { get; set; }
        public Question Question { get; set; }

        public int SelectedAnswer { get; set; }
        public int Score { get; set; }
    }

    public class BattleServiceFactory
    {
        private readonly DataContext _dataContext;

        public BattleServiceFactory(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<BattleService> CreateOneToOneBattleService(Battle battle, CancellationToken cancellationToken)
        {
            switch (battle.BattleType)
            {
                case BattleType.OneOnOne:
                    return new OneToOneBattleService(_dataContext, (OneToOneBattle)battle);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public abstract class BattleService
    {
        public Battle Battle { get; set; }

        public BattleService(Battle battle)
        {
            Battle = battle;
        }
    }

    public class OneToOneBattleService : BattleService
    {
        private readonly DataContext _dataContext;
        private readonly OneToOneBattle _battle;

        public OneToOneBattleService(DataContext dataContext, OneToOneBattle battle) : base(battle)
        {
            _dataContext = dataContext;
            _battle = battle;
        }
    }
}