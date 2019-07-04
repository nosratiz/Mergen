using Mergen.Core.Entities.Base;
using Newtonsoft.Json;

namespace Mergen.Core.Entities
{
    public class Level : Entity
    {
        [JsonProperty("Level")]
        public int LevelNumber { get; set; }

        [JsonProperty("MinScore")]
        public int MinScore { get; set; }

        [JsonProperty("MaxScore")]
        public int? MaxScore { get; set; }
    }
}