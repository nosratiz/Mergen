using Mergen.Core.Entities;

namespace Mergen.Game.Api.API.Accounts
{
    public class SessionViewModel : EntityViewModel
    {
        public string AccessToken { get; set; }
        public string AccountId { get; set; }

        public static SessionViewModel Map(Session session)
        {
            return new SessionViewModel
            {
                Id = session.Id,
                AccessToken = session.AccessToken,
                AccountId = session.AccountId.ToString()
            };
        }
    }
}