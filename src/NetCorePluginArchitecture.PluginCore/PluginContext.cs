using System;
using System.Collections.Generic;

namespace NetCorePluginArchitecture.PluginCore
{
    public class PluginContext : IDisposable
    {
        public IList<PluginInfo> Plugins { get; set; } = new List<PluginInfo>();

        public void Dispose()
        {
            Plugins.Clear();
            Plugins = null;
        }
    }
}