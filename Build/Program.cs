using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Build.Projects;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core.IO;
using Microsoft.Extensions.Configuration;
using Path = System.IO.Path;

namespace Build
{
    class Program
    {
        static  async Task Main(string[] args)
        {
            var target = CakeBridge.Context.Argument<string>("target", null);
            var dbConnectionString = CakeBridge.Context.Argument<string>("dbc", null);

            Console.WriteLine(target);

            CakeBridge.Setup(context =>
            {
                context.Information("Setting up...");

                var buildPath = AppDomain.CurrentDomain.BaseDirectory;
                var rootPath = context.Directory(".").Path.MakeAbsolute(DirectoryPath.FromString(buildPath)).Combine(@"..\..");
/*

                var buildSettingsConfigBuilder = new ConfigurationBuilder().SetBasePath(buildPath)
                    .AddJsonFile(Path.Combine(buildPath, "buildsettings.json"))*/

                return new BuildData
                {
                    RootPath = rootPath,
                    ApplicationsSourcePath = rootPath.Combine(@"MergenAPI"),
                    ApplicationsBinPath = rootPath.Combine(@"Projects\Application\Bin"),
                    ApplicationsPublishPath = rootPath.Combine(@"Bin\Publish"),
                    DatabaseSourcePath = rootPath.Combine(@"Database"),
                    DatabaseBinPath = rootPath.Combine(@"Database\Bin"),
                    DatabasePublishPath = rootPath.Combine(@"Database\Bin\Publish"),
                };
            });

            CakeBridge.Teardown(context =>
            {
                context.Information("Tearing down...");
            });

            var projects = new List<IProject>
            {
                new GameApi(),
                new AdminApi()
            };

            foreach (var project in projects)
            {
                project.AddTasks();
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                Console.Error.WriteLine("No targets specified. available targets are: {0}", string.Join(", ", CakeBridge.Tasks.Select(q => q.Name)));
                return;
            }

            await Task.WhenAll(target.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(CakeBridge.RunTargetAsync));
        }
    }
}