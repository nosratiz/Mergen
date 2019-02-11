using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Data
{
    public class Battle : Entity
    {
        public BattleType BattleType { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}