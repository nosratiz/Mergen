using System;

namespace Mergen.Admin.Api.Data.Entities
{
	public class UserInvitation
	{
		public int UserId { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime InvitationDateTime { get; set; }
		public int StatusId { get; set; }
		public int RegisteredUserId { get; set; }
	}
}