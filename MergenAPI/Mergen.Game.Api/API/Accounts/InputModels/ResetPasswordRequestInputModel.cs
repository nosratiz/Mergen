using System.ComponentModel.DataAnnotations;

namespace Mergen.Game.Api.API.Accounts.InputModels
{
    public class ResetPasswordRequestInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}