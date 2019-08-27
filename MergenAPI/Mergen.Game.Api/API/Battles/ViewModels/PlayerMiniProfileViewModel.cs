namespace Mergen.Game.Api.API.Battles.ViewModels
{
    public class PlayerMiniProfileViewModel
    {
        public string AvatarAccountId { get; set; }
        public string Name { get; }
        public int Level { get; }

        public PlayerMiniProfileViewModel(string name, int level, string avatarAccountId)
        {
            Name = name;
            Level = level;
            AvatarAccountId = avatarAccountId;
        }
    }
}