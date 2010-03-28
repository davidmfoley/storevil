using System.Collections.Generic;
using System.IO;
using System.Linq;
using StorEvil.Configuration;
using StorEvil.Infrastructure;

namespace StorEvil.Parsing
{
    /// <summary>
    /// returns all of the stories/specs from a directory
    /// </summary>
    public class FilesystemStoryReader : IStoryReader
    {
        private readonly ConfigSettings Settings;

        public FilesystemStoryReader(IFilesystem filesystem, ConfigSettings settings)
        {
            Settings = settings;
            Filesystem = filesystem;
        }

        public IStoryParser Parser { get; set; }
        public IFilesystem Filesystem { get; set; }

        public IEnumerable<StoryInfo> GetStoryInfos()
        {
            return GetStoryInfos(Settings.StoryBasePath);
        }

        private IEnumerable<StoryInfo> GetStoryInfos(string path)
        {
            if (Filesystem.FileExists(path))
            {
                return new[] {GetStoryInfo(path)};
            }
            var stories = Filesystem
                .GetFilesInFolder(path)
                .Where(ExtensionIsSupportedByCurrentSettings)
                .Select(file =>GetStoryInfo(file))                       
                .ToList();

            foreach (var subPath in Filesystem.GetSubFolders(path))
                stories.AddRange(GetStoryInfos(subPath));

            return stories;
        }

        private StoryInfo GetStoryInfo(string file)
        {
            return new StoryInfo
                       {
                           Id = Path.GetFileNameWithoutExtension(file),
                           Text = Filesystem.GetFileText(file)
                       };
        }

        private bool ExtensionIsSupportedByCurrentSettings(string file)
        {
            if (Settings.ScenarioExtensions == null || ! Settings.ScenarioExtensions.Any())
                return true;

            var extension = Path.GetExtension(file);
            return Settings.ScenarioExtensions.Any(x => extension == x);
        }
    }
}