using Mergen.Core.Entities;
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
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AccountCategoryStat> AccountCategoryStats { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<AccountFriend> AccountFriends { get; set; }
    }
}