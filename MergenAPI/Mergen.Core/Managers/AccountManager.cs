using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Helpers;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Managers
{
    public class AccountManager : EntityManagerBase<Account>
    {
        public AccountManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory,
            queryProcessor)
        {
        }

        public async Task<Account> FindByEmailAsync(string emailAddress, CancellationToken cancellationToken = default)
        {
            return await this.FirstOrDefaultAsync(q => q.NormalizedEmail == emailAddress.NormalizeEmail(),
                cancellationToken);
        }

        public async Task<Account> FindByResetPasswordTokenAsync(string resetPasswordToken,
            CancellationToken cancellationToken)
        {
            return await FirstOrDefaultAsync(q => q.ResetPasswordToken == resetPasswordToken, cancellationToken);
        }

        public async Task UpdateRolesAsync(Account account, IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                dbc.AccountRoles.RemoveRange(dbc.AccountRoles.Where(q => q.AccountId == account.Id));

                foreach (var roleId in roleIds)
                {
                    dbc.AccountRoles.Add(new AccountRole
                    {
                        AccountId = account.Id,
                        RoleId = roleId
                    });
                }

                await dbc.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<long>> GetRolesAsync(Account account, CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                return await dbc.AccountRoles.Where(q => q.AccountId == account.Id).Select(q => q.RoleId).ToListAsync(cancellationToken: cancellationToken);
            }
        }

        public async Task<IEnumerable<(Account account, AccountStatsSummary stats)>> SearchAsync(string term, int page = 1, int pageSize = 30, CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                //var result = await dbc.Accounts.Where(q => q.Nickname.Contains(term))
                //    .Join(dbc.AccountStatsSummaries, account => account.Id, summary => summary.AccountId,
                //        (account, summary) => new {account, summary})
                //    .ToListAsync(cancellationToken);

                var query = from acc in dbc.Accounts
                    join statsSummary in dbc.AccountStatsSummaries on acc.Id equals statsSummary.AccountId
                        into statSummaries
                    from stats in statSummaries.DefaultIfEmpty()
                    select new {acc, stats};

                return (await query.Skip((page - 1) * pageSize).Take(page * pageSize).ToListAsync(cancellationToken)).Select(q => (q.acc, q.stats));
            }
        }
    }
}