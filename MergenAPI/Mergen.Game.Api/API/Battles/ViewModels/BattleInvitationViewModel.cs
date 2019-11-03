using Mergen.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mergen.Game.Api.API.Battles.ViewModels
{
    public class BattleInvitationViewModel : EntityViewModel
    {
        public long InviterAccountId { get; set; }
        public DateTime DateTime { get; set; }

        public static BattleInvitationViewModel Map(BattleInvitation battleInvitation)
        {
            return new BattleInvitationViewModel
            {
                Id = battleInvitation.Id,
                InviterAccountId = battleInvitation.InviterAccountId,
                DateTime = battleInvitation.DateTime
            };
        }

        public static IEnumerable<BattleInvitationViewModel> MapAll(IEnumerable<BattleInvitation> battleInvitations)
        {
            return battleInvitations.Select(Map);
        }
    }
}