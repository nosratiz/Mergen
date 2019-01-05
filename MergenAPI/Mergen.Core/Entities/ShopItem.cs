using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class ShopItem : Entity
    {
        public string Title { get; set; }
        public int TypeId { get; set; }
        public int? AvatarCategoryId { get; set; }
        public int PriceTypeId { get; set; }
        public decimal Price { get; set; }
        public string ImageFileId { get; set; }
        public int? UnlockLevel { get; set; }
        public int? UnlockSky { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
    }

    public class PurchaseLog : Entity
    {
        public long AccountId { get; set; }
        public long ShopItemId { get; set; }
        public DateTime DateTime { get; set; }
        public long PurchasedByAccountId { get; set; }
    }

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