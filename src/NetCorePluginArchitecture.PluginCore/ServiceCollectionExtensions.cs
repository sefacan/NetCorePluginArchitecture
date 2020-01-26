using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Hosting;
using NetCorePluginArchitecture.PluginCore;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private const string pluginManifestName = "plugin.json";
        private const string pluginFolderName = "Plugins";
        private const string assemblyFileExtension = ".dll";

        public static IMvcBuilder AddPluginFutures(this IMvcBuilder mvcBuilder, IHostEnvironment environment)
        {
            var pluginContext = new PluginContext();

            var pluginsFolder = Path.Combine(environment.ContentRootPath, pluginFolderName);
            foreach (var dir in Directory.GetDirectories(pluginsFolder))
            {
                var pluginFolder = Path.GetFileName(dir);
                var pluginAssemblyPath = Path.Combine(dir, pluginFolder + assemblyFileExtension);
                if (!File.Exists(pluginAssemblyPath))
                    continue;

                var pluginManifestFile = Path.Combine(dir, pluginManifestName);
                if (!File.Exists(pluginManifestFile))
                    continue;

                PluginInfo pluginInfo = null;
                using (var reader = new StreamReader(pluginManifestFile))
                {
                    string content = reader.ReadToEnd();
                    pluginInfo = JsonSerializer.Deserialize<PluginInfo>(content);
                }

                if (pluginInfo != null)
                {
                    var pluginLoadContext = new PluginLoadContext(pluginAssemblyPath);

                    pluginInfo.Assembly = pluginLoadContext.LoadDefaultAssembly();
                    pluginContext.Plugins.Add(pluginInfo);

                    var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginInfo.Assembly);
                    foreach (var part in partFactory.GetApplicationParts(pluginInfo.Assembly))
                    {
                        mvcBuilder.PartManager.ApplicationParts.Add(part);
                    }

                    // This piece finds and loads related parts, such as WebPlugin1.Views.dll
                    var relatedAssembliesAttrs = pluginInfo.Assembly.GetCustomAttributes<RelatedAssemblyAttribute>();
                    foreach (var attr in relatedAssembliesAttrs)
                    {
                        var assembly = pluginLoadContext.LoadFromAssemblyName(new AssemblyName(attr.AssemblyFileName));
                        partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                        foreach (var part in partFactory.GetApplicationParts(assembly))
                        {
                            mvcBuilder.PartManager.ApplicationParts.Add(part);
                        }
                    }
                }
            }

            mvcBuilder.Services.AddSingleton(pluginContext);

            return mvcBuilder;
        }
    }
}