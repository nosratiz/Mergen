using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mergen.Game.Api.API.Accounts.ViewModels;

namespace Mergen.Game.Api.API.Accounts.InputModels
{
    public class SetAvatarInputModel
    {
        public List<Avatar> AvatarItemIds { get; set; }
    }
}