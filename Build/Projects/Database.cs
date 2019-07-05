using System;
using System.IO;
using System.Text;
using Cake.Core;

namespace Build.Projects
{
    public class Database : IProject
    {
        public string Name => "Database";
        public string ProjectSourceDir => "Database";

        public void AddTasks()
        {
            this.Task("Build").Does((Action<ICakeContext, BuildData>)Build);
        }

        private void Build(ICakeContext context, BuildData buildData)
        {
            var tablesPath = Path.Combine(ProjectSourceDir, "Tables");
            var dataPath = Path.Combine(ProjectSourceDir, "Data");

            var installScript = new StringBuilder();

            var tableFiles = Directory.GetFiles(tablesPath, "*.sql");
            foreach (var tableFile in tableFiles)
            {
                installScript.AppendLine(File.ReadAllText(tableFile));
                installScript.AppendLine("GO");
            }

            var dataFiles = Directory.GetFiles(dataPath, "*.sql");
            foreach (var dataFile in dataFiles)
            {
                installScript.AppendLine(dataFile);
                installScript.AppendLine("GO");
            }

            var installScriptPath = buildData.DatabaseBinPath.CombineWithFilePath("Install.sql");
            File.WriteAllText(installScriptPath.FullPath, installScript.ToString());

            var updateScript = new StringBuilder();
            var updatesPath = buildData.DatabaseSourcePath.Combine("Updates");
            var updateFiles = Directory.GetFiles(updatesPath.FullPath, "*.sql");
            foreach (var updateFile in updateFiles)
            {
                updateScript.AppendLine(File.ReadAllText(updateFile));
                updateScript.AppendLine("GO");
            }
        }

        private void Deploy(ICakeContext context, BuildData buildData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN TRAN");
            sb.AppendLine("DECLARE @__BuildInfoExists BIT = ISNULL((SELECT TOP 1 CAST(1 AS BIT) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Build' AND  TABLE_NAME = 'Info'), 0);");
            sb.AppendLine("IF (@__BuildInfoExists)");
        }

        private class BuildInfo
        {
            public int Version { get; set; }
            public string DateTime { get; set; }
        }
    }
}