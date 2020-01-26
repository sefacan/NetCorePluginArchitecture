using NetCorePluginArchitecture.PluginCore;

namespace WebPlugin1
{
    public class WebPluginClass : IPlugin
    {
        public string GetPluginName() => "First Web Plugin From Interface";
    }
}
