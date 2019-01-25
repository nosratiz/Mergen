using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public DbSet<Game> Games { get; set; }
        public DbSet<OneToOneBattle> OneToOneBattles { get; set; }
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
        public Account Player1 { get; set; }
        public Account Player2 { get; set; }
        public int Round { get; set; }
        public ICollection<Game> Games { get; set; }
    }

    public enum BattleType
    {
        OneOnOne
    }

    public class Game : Entity
    {
        public long PlayerId { get; set; }
        public Account Player { get; set; }

        public long CategoryId { get; set; }
        public Category SelectedCategory { get; set; }

        public ICollection<GameCategory> GameCategories { get; set; }
        public ICollection<GameQuestion> GameQuestions { get; set; }
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
}