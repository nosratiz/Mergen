using System;
using System.Collections.Generic;
using System.IO;
using Mergen.Core;
using Mergen.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Mergen.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            services.RegisterMergenServices(configuration, true);
            var sp = services.BuildServiceProvider();
            ApplicationEvents.ApplicationStart(sp, configuration);

            var db = sp.GetRequiredService<DataContext>();
        }
    }
}
