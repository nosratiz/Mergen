using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;

namespace Mergen.Core.Managers
{
    public class AccountItemManager : EntityManagerBase<AccountItem>
    {
        public AccountItemManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public Task<IEnumerable<AccountItem>> GetByAccountIdAsync(long accountId, CancellationToken cancellationToken)
        {
            return GetAsync(q => q.AccountId == accountId, cancellationToken);
        }
    }
}