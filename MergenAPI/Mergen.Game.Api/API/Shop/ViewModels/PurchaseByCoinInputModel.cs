using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Shop.ViewModels
{
    public class PurchaseByCoinInputModel
    {
        [Required]
        public long ShopItemId { get; set; }
    }
}