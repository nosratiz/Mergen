using System;
using System.Collections.Generic;
using System.Linq;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;
using Newtonsoft.Json;

namespace Mergen.Admin.Api.API.Accounts.ViewModels
{
    public class AccountViewModel : EntityViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string StatusId { get; set; }
        public string StatusNote { get; set; }
        public string AvatarImageId { get; set; }
        public string CoverImageId { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string Timezone { get; set; }
        public IEnumerable<string> AvatarItemIds { get; set; }
        public IEnumerable<string> RoleIds { get; set; }

        public static AccountViewModel Map(Account account)
        {
            var model = new AccountViewModel();
            model.Id = account.Id.ToString();
            model.Email = account.Email;
            model.PhoneNumber = account.PhoneNumber;
            model.FirstName = account.FirstName;
            model.LastName = account.LastName;
            model.NickName = account.Nickname;
            model.GenderId = account.GenderId?.ToString();
            model.BirthDate = account.BirthDate;
            model.StatusId = account.StatusId.ToString();
            model.StatusNote = account.StatusNote;
            model.IsEmailVerified = account.IsEmailVerified;
            model.IsPhoneNumberVerified = account.IsPhoneNumberVerified;
            model.CoverImageId = account.CoverImageId;
            model.AvatarImageId = account.AvatarImageId;
            model.Timezone = account.Timezone;
            model.AvatarItemIds = account.AvatarItemIds != null ? JsonConvert.DeserializeObject<long[]>(account.AvatarItemIds).Select(q => q.ToString()) : new string[0];
            model.RoleIds = account.RoleIds != null ? JsonConvert.DeserializeObject<long[]>(account.RoleIds).Select(q=>q.ToString()) : new string[0];
            return model;
        }


        public static IEnumerable<AccountViewModel> MapAll(IEnumerable<Account> accounts)
        {
            return accounts.Select(Map);
        }
    }
}