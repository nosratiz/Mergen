using Cake.Core.IO;

namespace Build
{
    public class BuildData
    {
        public DirectoryPath RootPath { get; set; }
        public DirectoryPath ApplicationsSourcePath { get; set; }
        public DirectoryPath ApplicationsBinPath { get; set; }
        public DirectoryPath ApplicationsPublishPath { get; set; }
        public DirectoryPath DatabaseSourcePath { get; set; }
        public DirectoryPath DatabaseBinPath { get; set; }
        public DirectoryPath DatabasePublishPath { get; set; }
    }

    public static class BuildDataSourcePath
    {
        public static string GetProjectSourcePath(this BuildData buildData, string projectSourceDir)
        {
            return buildData.ApplicationsSourcePath.Combine(projectSourceDir).FullPath;
        }
    }
}