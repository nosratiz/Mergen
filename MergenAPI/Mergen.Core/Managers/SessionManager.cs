using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;

namespace Mergen.Core.Managers
{
    public class SessionManager : EntityManagerBase<Session>
    {
        public SessionManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory,
            queryProcessor)
        {
        }

        public async Task DeleteByAccountIdAsync(long accountId, CancellationToken cancellationToken)
        {
            foreach (var session in await GetAsync(q=>q.AccountId == accountId, cancellationToken))
            {
                await DeleteAsync(session, cancellationToken);
            }
        }
    }
}