using System;
using System.Reflection;

namespace NetCorePluginArchitecture.PluginCore
{
    public class PluginInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Version Version { get; set; }
        public Assembly Assembly { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }
}