using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace NetCorePluginArchitecture.MSBuildTask
{
    public class CopyModuleTask : Task
    {
        private readonly string pluginFileName = "plugin.json";

        [Required]
        public string ProjectDir { get; set; }

        [Required]
        public string PluginDir { get; set; }

        [Required]
        public string BuildConfiguration { get; set; }

        [Required]
        public string TargetFramework { get; set; }

        public override bool Execute()
        {
            foreach (var dir in Directory.GetDirectories(PluginDir))
            {
                if (!File.Exists(Path.Combine(dir, pluginFileName)))
                    continue;

                var sourceDir = Path.Combine(dir, "bin", BuildConfiguration, TargetFramework);
                var destinationDir = Path.Combine(ProjectDir, "Plugins", dir);
                CopyDirectory(sourceDir, destinationDir);

                Log.LogMessage(MessageImportance.High, $"Copied plugin {new DirectoryInfo(dir).Name}");
            }

            return true;
        }

        private void CopyDirectory(string sourcePath, string targetPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                return;
            }

            CopyAll(new DirectoryInfo(sourcePath), new DirectoryInfo(targetPath));
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (var subDirectory in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(subDirectory.Name);
                CopyAll(subDirectory, nextTargetSubDir);
            }
        }
    }
}
