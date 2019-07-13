using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Accounts
{
    public class AchievementTypeViewModel : EntityViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageFileId { get; set; }

        public static AchievementTypeViewModel Map(AchievementType achievementType)
        {
            return new AchievementTypeViewModel
            {
                Id = achievementType.Id,
                Title = achievementType.Title,
                Description = achievementType.Description,
                ImageFileId = achievementType.ImageFileId
            };
        }
    }
}