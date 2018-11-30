using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.Achievements
{
    public class AchievementViewModel
    {
        public string AccountId { get; set; }
        public string AchievementTypeId { get; set; }
        public DateTime AchieveDateTime { get; set; }

        public static AchievementViewModel Map(Achievement achievement)
        {
            return AutoMapper.Mapper.Map<AchievementViewModel>(achievement);
        }

        public static IEnumerable<AchievementViewModel> MapAll(IEnumerable<Achievement> achievements)
        {
            return achievements.Select(Map);
        }
    }
}