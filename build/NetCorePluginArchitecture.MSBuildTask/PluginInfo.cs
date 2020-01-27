using System;

namespace NetCorePluginArchitecture.MSBuildTask
{
    public class PluginInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Version Version { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }
}