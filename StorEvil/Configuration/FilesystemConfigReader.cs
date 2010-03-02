using System;
using System.IO;
using System.Linq;

namespace StorEvil
{
    public class FilesystemConfigReader : IConfigSource
    {
        private readonly IFilesystem _filesystem;
        private readonly IConfigParser _parser;

        public FilesystemConfigReader(IFilesystem filesystem, IConfigParser parser)
        {
            _filesystem = filesystem;
            _parser = parser;
        }

        public ConfigSettings GetConfig(string directoryOrFile)
        {
            var path = directoryOrFile;
            if (!path.EndsWith("\\"))
                path += "\\";  

            var containingDirectory =  Path.GetDirectoryName(Path.GetFullPath(path));

            while (containingDirectory.Length > Path.GetPathRoot(containingDirectory).Length)
            {
                var configLocation = Path.Combine(containingDirectory, "storevil.config");

                if (_filesystem.FileExists(configLocation))
                {
                    var fileContents = _filesystem.GetFileText(configLocation);
                    var config = _parser.Read(fileContents);
                    FixUpPaths(Path.GetDirectoryName(configLocation), config);
                    if (config.StoryBasePath == null)
                        config.StoryBasePath = path;
                    return config;  
                }

                var parent = Directory.GetParent(containingDirectory);
                if (parent == null)
                    return ConfigSettings.Default();

                containingDirectory = parent.FullName;                
            }

            return ConfigSettings.Default();
        }

        private void FixUpPaths(string basePath, ConfigSettings settings)
        {
            settings.AssemblyLocations = settings.AssemblyLocations.Select(x => FixUpPath(basePath, x));
        }

        private string FixUpPath(string basePath, string path)
        {
            if (Path.IsPathRooted(path))
                return path;    
            return Path.Combine(basePath, path);
        }
    }
}