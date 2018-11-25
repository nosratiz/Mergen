using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountAchievement : Entity
    {
        public int AccountId { get; set; }
        public int AchievementId { get; set; }
        public DateTime AchieveDateTime { get; set; }
    }
}