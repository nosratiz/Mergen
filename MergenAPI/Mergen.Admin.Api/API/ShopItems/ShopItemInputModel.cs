using System.ComponentModel.DataAnnotations;

namespace Mergen.Admin.Api.API.ShopItems
{
    public class ShopItemInputModel
    {
        public string Title { get; set; }
        [Required]
        public string TypeId { get; set; }
        public string AvatarCategoryId { get; set; }
        [Required]
        public string PriceTypeId { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string ImageFileId { get; set; }
        public int? UnlockLevel { get; set; }
        public int? UnlockSky { get; set; }
        [Required]
        public string StatusId { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public string AvatarTypeId { get; set; }
        public bool? DefaultAvatar { get; set; }
    }
}