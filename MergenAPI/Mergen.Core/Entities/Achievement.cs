using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Achievement : Entity
    {
        public long AccountId { get; set; }
        public long AchievementTypeId { get; set; }
        public AchievementType AchievementType { get; set; }
        public DateTime AchieveDateTime { get; set; }
    }
}