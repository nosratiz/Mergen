using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Payment : Entity
    {
        public Guid UniqueId { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        public decimal Amount { get; set; }
        public long ShopItemId { get; set; }
        public ShopItem ShopItem { get; set; }
        public PaymentState State { get; set; }
        public string RedirectUrl { get; set; }
    }
}
