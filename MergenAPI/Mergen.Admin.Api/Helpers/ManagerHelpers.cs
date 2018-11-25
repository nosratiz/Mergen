using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.Exceptions;
using Mergen.Core.Entities.Base;
using Mergen.Core.Managers.Base;

namespace Mergen.Admin.Api.Helpers
{
    public static class ManagerHelpers
    {
        public static async Task<TEntity> GetByIdAsyncThrowNotFoundIfNotExists<TEntity>(this EntityManagerBase<TEntity> manager, string id, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            var entity = await manager.GetAsync(id.ToLong(), cancellationToken);

            if (entity == null)
                throw new NotFoundException();

            return entity;
        }
    }
}
