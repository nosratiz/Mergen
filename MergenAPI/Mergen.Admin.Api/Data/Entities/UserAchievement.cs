using System;

namespace Mergen.Admin.Api.Data.Entities
{
	public class UserAchievement
	{
		public int UserId { get; set; }
		public int AchievementId { get; set; }
		public DateTime AchieveDateTime { get; set; }
	}
}