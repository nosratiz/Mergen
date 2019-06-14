using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mergen.Core.Options
{
    public class GameSettings
    {
        public int SelectCustomCategoryPrice { get; set; }
        public int RandomizeCategoryPrice { get; set; }
        public int RemoveTwoAnswersHelperPrice { get; set; }
        public int AnswersHistoryHelperPrice { get; set; }
        public int AskMergenHelperPrice { get; set; }
        public int TimeExtenderHelperPrice { get; set; }
        public int DoubleChanceHelperPrice { get; set; }
    }
}