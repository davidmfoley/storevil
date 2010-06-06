using JetBrains.ProjectModel;
using StorEvil.Configuration;
using StorEvil.Infrastructure;

namespace StorEvil.Resharper
{
    internal class StorEvilResharperConfigProvider
    {
        public ConfigSettings GetConfigSettingsForProject(IProject project)
        {
            var reader = new FilesystemConfigReader(new Filesystem(), new ConfigParser());

            return GetConfigForProject(project, reader);
        }

        private static ConfigSettings GetConfigForProject(IProject project, FilesystemConfigReader reader)
        {
            if (project.ProjectFile == null)
                return null;

            var location = project.ProjectFile.ParentFolder.Location;

            if (string.IsNullOrEmpty(location.FullPath))
                return null;

            return reader.GetConfig(location.FullPath);
        }
    }
}