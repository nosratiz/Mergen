using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mergen.Core.Data
{
    public class DbContextFactory
    {
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public DbContextFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _dbContextOptions = serviceScopeFactory.CreateScope().ServiceProvider
                .GetRequiredService<DbContextOptions<DataContext>>();
        }

        public DataContext CreateDbContext()
        {
            return new DataContext(_dbContextOptions);
        }
    }
}