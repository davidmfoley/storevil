using System.IO;

namespace StorEvil
{
    public class FilesystemConfigReader : IConfigSource
    {
        private readonly IFilesystem _filesystem;
        private readonly IConfigParser _parser;

        public FilesystemConfigReader(IFilesystem filesystem, IConfigParser reader)
        {
            _filesystem = filesystem;
            _parser = reader;
        }

        public ConfigSettings GetConfig(string directoryOrFile)
        {
            var containingDirectory = Path.GetFullPath(Path.GetDirectoryName(directoryOrFile));

            while (containingDirectory.Length > Path.GetPathRoot(containingDirectory).Length)
            {
                var configLocation = Path.Combine(containingDirectory, "storevil.config");

                if (_filesystem.FileExists(configLocation))
                {
                    var fileContents = _filesystem.GetFileText(configLocation);
                    return _parser.Read(fileContents);
                }

                var parent = Directory.GetParent(containingDirectory);
                if (parent == null)
                    return ConfigSettings.Default();

                containingDirectory = parent.FullName;                
            }

            return ConfigSettings.Default();
        }
    }
}