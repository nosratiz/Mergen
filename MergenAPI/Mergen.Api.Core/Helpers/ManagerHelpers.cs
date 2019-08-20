using System.Threading;
using System.Threading.Tasks;
using Mergen.Api.Core.Exceptions;
using Mergen.Core.Entities.Base;
using Mergen.Core.Managers.Base;

namespace Mergen.Api.Core.Helpers
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
