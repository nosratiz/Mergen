namespace Mergen.Game.Api.API.Shop
{
    public class ShopItemFilterInputModel
    {
        public string Title { get; set; }
        public string TypeId { get; set; }
        public string PriceTypeId { get; set; }
        public decimal Price { get; set; }
        public string StatusId { get; set; }
        public string AvatarCategoryId { get; set; }
    }
}