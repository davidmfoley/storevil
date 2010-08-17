using System.IO;
using JetBrains.ProjectModel;
using StorEvil.Configuration;
using StorEvil.Infrastructure;

namespace StorEvil.Resharper
{
    internal class StorEvilResharperConfigProvider
    {
        public ConfigSettings GetConfigSettingsForProject(string projectFile)
        {
            var reader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());

            return GetConfigForProject(projectFile, reader);
        }

        private static ConfigSettings GetConfigForProject(string projectFile, FilesystemConfigReader reader)
        {
            if (string.IsNullOrEmpty(projectFile))
                return null;

            return reader.GetConfig(Directory.GetParent(projectFile).FullName);
        }
    }
}