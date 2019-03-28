using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Accounts
{
    public class LoginInputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}