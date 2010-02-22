using System.Collections.Generic;
using System.IO;

namespace StorEvil
{
    public class ConfigSettings
    {
        public IEnumerable<string> ScenarioExtensions { get; set; }
        public IEnumerable<string> AssemblyLocations { get; set; }

        public string StoryBasePath { get; set; }

        public static ConfigSettings Default()
        {
            return new ConfigSettings
                       {
                           ScenarioExtensions = new [] {".txt", ".feature", ".story"}, 
                           AssemblyLocations = new string[0]
                       };
        }
    }
}