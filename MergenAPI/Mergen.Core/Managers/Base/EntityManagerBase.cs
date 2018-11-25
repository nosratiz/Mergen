using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Managers.Base
{
    public abstract class EntityManagerBase<TEntity> where TEntity : Entity
    {
        private readonly DbContextFactory _dbContextFactory;
        private protected readonly QueryProcessor QueryProcessor;

        protected EntityManagerBase(DbContextFactory dbContextFactory, QueryProcessor queryProcessor)
        {
            _dbContextFactory = dbContextFactory;
            QueryProcessor = queryProcessor;
        }

        protected DataContext CreateDbContext()
        {
            return _dbContextFactory.CreateDbContext();
        }

        protected async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using (var db = CreateDbContext())
            {
                db.Set<TEntity>().Add(entity);
                await db.SaveChangesAsync(cancellationToken);
                return entity;
            }
        }

        protected async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                db.Set<TEntity>().Update(entity);
                await db.SaveChangesAsync(cancellationToken);
                return entity;
            }
        }

        public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == default)
                return InsertAsync(entity, cancellationToken);
            else
                return UpdateAsync(entity, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using (var db = CreateDbContext())
            {
                return await db.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
            }
        }

        public async Task<QueryResult<TEntity>> GetAllAsync(QueryInputModel queryModel,
            CancellationToken cancellationToken = default)
        {
            using (var db = CreateDbContext())
            {
                return await QueryProcessor.ApplyAsync(db.Set<TEntity>().AsNoTracking(), queryModel, cancellationToken);
            }
        }

        public async Task<TEntity> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            using (var db = CreateDbContext())
            {
                return await db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            using (var db = CreateDbContext())
            {
                return await db.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
            }
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            using (var db = CreateDbContext())
            {
                return await db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
            }
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                dbc.Set<TEntity>().Remove(entity);
                await dbc.SaveChangesAsync(cancellationToken);
            }
        }
    }
}