using Mergen.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
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
    }
}