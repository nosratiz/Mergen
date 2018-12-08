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
    }
}