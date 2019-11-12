using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Managers
{
    public class NotificationManager : EntityManagerBase<Notification>
    {
        public NotificationManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public async Task<List<Notification>> GetNotificationListAsync(long accountId,
            CancellationToken cancellationToken)
        {
            using (var dbc = CreateDbContext())
            {
                return await dbc.Notifications.Where(x => x.IsArchived == false && x.AccountId == accountId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<Notification> GEtNotificationAsync(long id, long accountId, CancellationToken cancellationToken)
        {
            using (var dbc = CreateDbContext())
            {
                return await dbc.Notifications.FirstOrDefaultAsync(
                    x => x.IsArchived == false && x.Id == id && x.AccountId == accountId,
                    cancellationToken: cancellationToken);
            }
        }
    }
}