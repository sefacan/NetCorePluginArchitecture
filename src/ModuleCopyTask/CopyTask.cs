using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ModuleCopyTask
{
    public class CopyTask : Task
    {
        private const string moduleFileName = "module.json";

        public string PluginProjectsDir { get; set; }

        public override bool Execute()
        {
            foreach (var dir in Directory.GetDirectories(PluginProjectsDir))
            {

            }

            return true;
        }
    }
}
