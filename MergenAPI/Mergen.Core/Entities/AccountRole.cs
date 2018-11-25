using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountRole : Entity
    {
        public long AccountId { get; set; }
        public long RoleId { get; set; }
    }
}