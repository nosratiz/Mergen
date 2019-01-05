using System;
using System.Collections.Generic;
using Mergen.Core.Entities.Base;
using Mergen.Core.Helpers;

namespace Mergen.Core.Entities
{
    public class Account : Entity
    {
        private string _email;

        public string Email
        {
            get => _email;
            set
            {
                NormalizedEmail = value.NormalizeEmail();
                _email = value;
            }
        }

        public string NormalizedEmail { get; set; }

        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public int? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public int StatusId { get; set; }
        public string StatusNote { get; set; }
        public string AvatarImageId { get; set; }
        public string CoverImageId { get; set; }
        public string PasswordHash { get; set; }

        public string EmailVerificationToken { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? EmailVerificationTokenGenerationTime { get; set; }
        public string PhoneNumberVerificationToken { get; set; }
        public DateTime? PhoneNumberVerificationTokenGenerationTime { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenGenerationTime { get; set; }
        public string Timezone { get; set; }


        // Cache Items
        public string AvatarItemIds { get; set; }
        public string RoleIds { get; set; }
    }
}