using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mergen.Core.Data;
using Newtonsoft.Json;

namespace Mergen.Core.Managers
{
    public class LevelManager
    {
        private readonly Lazy<IList<Level>> _levels = new Lazy<IList<Level>>(InitializeLevels);

        private static IList<Level> InitializeLevels()
        {
            var levelsStr = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "levels.json"));
            return JsonConvert.DeserializeObject<IList<Level>>(levelsStr);
        }

        private IList<Level> Levels => _levels.Value;

        public Level GetLevel(decimal score)
        {
            return Levels.FirstOrDefault(q => score >= q.MinScore && (score <= q.MaxScore || q.MaxScore == null));
        }
    }
}