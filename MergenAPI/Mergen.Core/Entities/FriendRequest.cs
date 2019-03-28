using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class FriendRequest : Entity
    {
        public long FromAccountId { get; set; }
        public long ToAccountId { get; set; }
        public DateTime RequestDateTime { get; set; }
        public FriendRequestStatus StatusId { get; set; }
    }
}