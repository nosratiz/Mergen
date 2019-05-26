using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API
{
    public class ResetPasswordInputModel
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}