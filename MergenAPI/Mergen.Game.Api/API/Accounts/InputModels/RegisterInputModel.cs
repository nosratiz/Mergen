using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Accounts.InputModels
{
    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}