using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Accounts.InputModels
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