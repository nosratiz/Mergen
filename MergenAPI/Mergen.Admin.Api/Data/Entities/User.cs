using System;

namespace Mergen.Admin.Api.Data.Entities
{
	public class User
	{
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string NickName { get; set; }
		public int? GenderId { get; set; }
		public DateTime BirthDate { get; set; }
		public int StatusId { get; set; }
		public string StatusNotes { get; set; }
		public string AvatarImageId { get; set; }
		public string CoverImageId { get; set; }
	}
}