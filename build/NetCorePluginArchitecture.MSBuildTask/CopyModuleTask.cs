using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NetCorePluginArchitecture.MSBuildTask
{
    public class CopyModuleTask : Task
    {
        private const string pluginManifestName = "plugin.json";

        [Required]
        public string PluginSourcePath { get; set; }

        [Required]
        public string PluginOutputPath { get; set; }

        [Required]
        public string BuildConfiguration { get; set; }

        [Required]
        public string TargetFramework { get; set; }

        public override bool Execute()
        {
            foreach (var dir in Directory.GetDirectories(PluginSourcePath))
            {
                var pluginFolder = Path.GetFileName(dir);
                var pluginAssemblyPath = Path.Combine(dir, pluginFolder + ".dll");
                if (!File.Exists(pluginAssemblyPath))
                    continue;

                var pluginManifestFile = Path.Combine(dir, pluginManifestName);
                if (!File.Exists(pluginManifestFile))
                    continue;

                PluginInfo pluginInfo = null;
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(pluginManifestFile))))
                {
                    var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(PluginInfo));
                    pluginInfo= dataContractJsonSerializer.ReadObject(ms) as PluginInfo;
                }

                if (pluginInfo == null)
                    continue;

                var sourceDir = Path.Combine(dir, "bin", BuildConfiguration, TargetFramework);
                var destinationDir = Path.Combine(PluginOutputPath, dir);

                CopyDirectory(sourceDir, destinationDir);

                Log.LogMessage(MessageImportance.High, $"Copied plugin {destinationDir}");
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
