using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Shop
{
    public class PurchaseByCoinInputModel
    {
        [Required]
        public long ShopItemId { get; set; }
    }
}