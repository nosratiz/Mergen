using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class BattleInvitation : Entity
    {
        public long InviterAccountId { get; set; }
        public Account InviterAccount { get; set; }
        public long AccountId { get; set; }
        public DateTime DateTime { get; set; }
        public BattleInvitationStatus Status { get; set; }
    }
}