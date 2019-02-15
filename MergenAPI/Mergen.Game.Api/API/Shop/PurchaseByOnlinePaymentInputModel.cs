using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Shop
{
    public class PurchaseByOnlinePaymentInputModel
    {
        [Required]
        public long ShopItemId { get; set; }

        [Required]
        public string RedirectUrl { get; set; }
    }
}