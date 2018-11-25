using System;
using System.ComponentModel.DataAnnotations;

namespace Mergen.Admin.Api.API.Accounts.InputModels
{
    public class AccountInputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string GenderTypeId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string StatusId { get; set; }
        public string StatusNote { get; set; }
        public string[] RoleIds { get; set; }

        public bool IsEmailVerified { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
    }
}