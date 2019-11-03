using Mergen.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Mergen.Game.Api.API.Shop.InputModels
{
    public class ShopItemViewModel : EntityViewModel
    {
        public string Title { get; set; }
        public int TypeId { get; set; }
        public int AvatarCategoryId { get; set; }
        public int? AvatarTypeId { get; set; }
        public bool? DefaultAvatar { get; set; }
        public string PriceTypeId { get; set; }
        public decimal Price { get; set; }
        public string ImageFileId { get; set; }
        public int? UnlockLevel { get; set; }
        public int? UnlockSky { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }

        public static ShopItemViewModel Map(ShopItem shopItem)
        {
            return AutoMapper.Mapper.Map<ShopItemViewModel>(shopItem);
        }

        public static IEnumerable<ShopItemViewModel> MapAll(IEnumerable<ShopItem> shopItems)
        {
            return shopItems.Select(Map);
        }
    }
}