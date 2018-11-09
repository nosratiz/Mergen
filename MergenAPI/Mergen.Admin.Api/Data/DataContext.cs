using Mergen.Admin.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Admin.Api.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
		}

		public DbSet<Achievement> Achievements { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<ShopItem> ShopItems { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserAchievement> UserAchievements { get; set; }
		public DbSet<UserInvitation> UserInvitations { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<UserStatsSummary> UserStatsSummaries { get; set; }
	}
}