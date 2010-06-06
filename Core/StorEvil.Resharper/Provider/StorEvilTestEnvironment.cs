using System.Collections.Generic;
using JetBrains.ProjectModel;

namespace StorEvil.Resharper
{
    internal class StorEvilTestEnvironment
    {
        private StorEvilResharperConfigProvider _configProvider = new StorEvilResharperConfigProvider();
        private Dictionary<string, StorEvilProject> _cache = new Dictionary<string, StorEvilProject>();

        public StorEvilProject GetProject(string  projectFile)
        {
            var key = projectFile;

            if (!_cache.ContainsKey(key))
                _cache.Add(key, new StorEvilProject(_configProvider.GetConfigSettingsForProject(projectFile)));

            return _cache[key];
        }
    }
}