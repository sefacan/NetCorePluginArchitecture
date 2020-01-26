using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace NetCorePluginArchitecture.PluginCore
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly string _basePath;
        private readonly string _mainAssemblyPath;
        private readonly AssemblyDependencyResolver _dependencyResolver;
        private AssemblyLoadContext _defaultLoadContext;

        public PluginLoadContext(string mainAssemblyPath)
            : base(mainAssemblyPath)
        {
            _basePath = Path.GetDirectoryName(mainAssemblyPath);
            _mainAssemblyPath = mainAssemblyPath;
            _dependencyResolver = new AssemblyDependencyResolver(_mainAssemblyPath);
            _defaultLoadContext = GetLoadContext(Assembly.GetExecutingAssembly()) ?? Default;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == null)
                return null;

            try
            {
                var defaultAssembly = _defaultLoadContext.LoadFromAssemblyName(assemblyName);
                if (defaultAssembly != null)
                {
                    return defaultAssembly;
                }
            }
            catch
            {
            }

            var resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath))
            {
                return LoadFromAssemblyPath(resolvedPath);
            }

            var localFile = Path.Combine(_basePath, assemblyName.Name + ".dll");
            if (File.Exists(localFile))
            {
                return LoadFromAssemblyPath(localFile);
            }

            return base.Load(assemblyName);
        }

        public Assembly LoadDefaultAssembly()
        {
            return LoadFromAssemblyPath(_mainAssemblyPath);
        }
    }
}