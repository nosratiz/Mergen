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
    }
}