using System.Collections.Generic;
using System.Linq;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.ShopItems
{
    public class ShopItemViewModel : EntityViewModel
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