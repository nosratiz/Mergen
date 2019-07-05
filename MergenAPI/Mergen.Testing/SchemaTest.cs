using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mergen.Core;
using Mergen.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestSupport.EfSchemeCompare;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Mergen.Testing
{
    public class SchemaTest
    {
        [Fact]
        public async Task CheckSchema()
        {
            IServiceCollection services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            services.RegisterMergenServices(configuration, true);
            var sp = services.BuildServiceProvider();
            ApplicationEvents.ApplicationStart(sp, configuration);

            using (var context = sp.GetRequiredService<DataContext>())
            {
                var comparer = new CompareEfSql();
                var hasErrors = comparer.CompareEfWithDb(context);
                hasErrors.ShouldBeFalse(comparer.GetAllErrors);
            }
        }
    }
}
