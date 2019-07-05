using System.IO;
using System.Linq;
using System.Threading;
using Cake.Common;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Build.Projects
{
    public class GameApi : IProject
    {
        public string Name => "GameAPI";
        public string ProjectSourceDir => "Mergen.Game.Api";

        public void AddTasks()
        {
            var buildTask = this.Task("Build")
                .Does<BuildData>((context, buildData) =>
                {
                    context.DotNetCoreBuild(buildData.GetProjectSourcePath(ProjectSourceDir));
                });

            var publishTask = this.Task("Publish")
                .Does<BuildData>((context, buildData) =>
                {
                    var publishPath = buildData.ApplicationsPublishPath.Combine("GameApi");

                    if (Directory.Exists(publishPath.FullPath))
                        Directory.Delete(publishPath.FullPath, true);

                    context.DotNetCorePublish(buildData.GetProjectSourcePath(ProjectSourceDir), new DotNetCorePublishSettings
                    {
                        OutputDirectory = publishPath,
                    });

                    var allPublishedFiles = Directory.GetFiles(publishPath.FullPath, "*.*",
                        SearchOption.AllDirectories);

                    File.WriteAllText(Path.Combine(publishPath.FullPath, "publish_files.txt"),
                        string.Join("\r\n", allPublishedFiles.Select(fp => Path.GetRelativePath(publishPath.FullPath, fp))));
                });

            var deployTask = this.Task("Deploy")
                .IsDependentOn(publishTask)
                .Does<BuildData>((context, buildData) =>
                {
                    var deployPath = context.Argument<string>("GameApi.DeployPath");
                    context.Log.Information($"DeployPath={deployPath}");

                    context.Log.Information("Creating app_offline.htm to stop the app...");
                    var appOfflinePath = Path.Combine(deployPath, "app_offline.htm");
                    File.WriteAllText(appOfflinePath, "Maintenance...");

                    context.Log.Information("Waiting 3000ms to make sure the app is stopped.");
                    Thread.Sleep(3000);

                    var publishedFilesPath = Path.Combine(deployPath, "publish_files.txt");
                    if (File.Exists(publishedFilesPath))
                    {
                        context.Log.Information("Deleting existing published files...");

                        var publishedFilesList = File.ReadAllText(publishedFilesPath).Split("\r\n");
                        foreach (var filePath in publishedFilesList)
                            File.Delete(Path.Combine(deployPath, filePath));
                    }

                    context.Log.Information("Copying application files...");
                    FileSystemHelpers.Copy(buildData.ApplicationsPublishPath.FullPath, deployPath);

                    context.Log.Information("Deleting app_offline.htm to make the app online ...");
                    File.Delete(appOfflinePath);
                });
        }
    }
}