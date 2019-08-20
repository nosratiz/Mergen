namespace Mergen.Game.Api.API.Battles.ViewModels
{
    public class PlayerMiniProfileViewModel
    {
        public string Name { get; }
        public int Level { get; }

        public PlayerMiniProfileViewModel(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }
}