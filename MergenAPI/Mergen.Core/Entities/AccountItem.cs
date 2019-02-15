using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class AccountItem : Entity
    {
        public long AccountId { get; set; }
        public long ShopItemId { get; set; }
        public int ItemTypeId { get; set; }
        public int Quantity { get; set; }
        public Account Account { get; set; }
        public ShopItem ShopItem { get; set; }
    }
}