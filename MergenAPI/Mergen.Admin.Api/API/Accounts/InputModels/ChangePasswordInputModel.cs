using System.ComponentModel.DataAnnotations;

namespace Mergen.Admin.Api.API.Accounts.InputModels
{
    public class ChangePasswordInputModel
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}