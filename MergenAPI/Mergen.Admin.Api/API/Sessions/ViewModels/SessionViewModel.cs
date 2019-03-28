using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;

namespace Mergen.Admin.Api.API.Sessions.ViewModels
{
    public class SessionViewModel : EntityViewModel
    {
        public string AccessToken { get; set; }
        public string AccountId { get; set; }

        public static SessionViewModel Map(Session session)
        {
            return new SessionViewModel
            {
                Id = session.Id.ToString(),
                AccessToken = session.AccessToken,
                AccountId = session.AccountId.ToString()
            };
        }
    }
}