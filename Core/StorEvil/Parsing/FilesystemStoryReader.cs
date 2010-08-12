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
        private readonly ConfigSettings _settings;

        public FilesystemStoryReader(IFilesystem filesystem, ConfigSettings settings)
        {
            _settings = settings;
            Filesystem = filesystem;
        }

        public IFilesystem Filesystem { get; set; }

        public IEnumerable<StoryInfo> GetStoryInfos()
        {
            return GetStoryInfos(_settings.StoryBasePath);
        }

        private IEnumerable<StoryInfo> GetStoryInfos(string path)
        {
            if (Filesystem.FileExists(path))
            {
                return new[] {GetStoryInfo(path)};
            }
            var filter = new FileExtensionFilter(_settings);
            var stories = Filesystem
                .GetFilesInFolder(path)
                .Where(filter.IsValid)
                .Select(GetStoryInfo)                       
                .ToList();

            foreach (var subPath in Filesystem.GetSubFolders(path))
                stories.AddRange(GetStoryInfos(subPath));

            return stories;
        }

        private StoryInfo GetStoryInfo(string file)
        {
            return new StoryInfo
                       {
                           Location = Path.GetFileName(file),
                           Text = Filesystem.GetFileText(file)
                       };
        }

      
    }
}