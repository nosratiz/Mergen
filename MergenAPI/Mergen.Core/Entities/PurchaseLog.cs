using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class PurchaseLog : Entity
    {
        public long AccountId { get; set; }
        public long ShopItemId { get; set; }
        public long Quantity { get; set; }
        public DateTime DateTime { get; set; }
        public long PurchasedByAccountId { get; set; }
    }
}