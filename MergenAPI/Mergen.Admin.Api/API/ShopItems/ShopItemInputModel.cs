namespace Mergen.Admin.Api.API.ShopItems
{
    public class ShopItemInputModel
    {
        public string Title { get; set; }
        public string TypeId { get; set; }
        public string AvatarCategoryId { get; set; }
        public string PriceTypeId { get; set; }
        public decimal Price { get; set; }
        public string ImageFileId { get; set; }
        public int? UnlockLevel { get; set; }
        public int? UnlockSky { get; set; }
        public string StatusId { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
    }
}