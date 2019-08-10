using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Battle : Entity
    {
        public BattleType BattleType { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? StartDateTime { get; set; }
    }
}