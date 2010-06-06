using System.Collections.Generic;
using System.IO;
using System.Linq;
using StorEvil.Configuration;
using StorEvil.Infrastructure;

namespace StorEvil.Parsing
{
    internal class FileExtensionFilter
    {
        private readonly ConfigSettings _settings;

        public FileExtensionFilter(ConfigSettings settings)
        {
            _settings = settings;
        }

        public bool IsValid(string file)
        {
            if (_settings.ScenarioExtensions == null || !_settings.ScenarioExtensions.Any())
                return true;

            var extension = Path.GetExtension(file);
            return _settings.ScenarioExtensions.Any(x => extension == x);
        }
    }

    public class SingleFileStoryReader : IStoryReader
    {
        private readonly IFilesystem _filesystem;
        private readonly ConfigSettings _settings;
        private readonly string _filename;

        public SingleFileStoryReader(IFilesystem filesystem, ConfigSettings settings, string filename)
        {
            _filesystem = filesystem;
            _settings = settings;
            _filename = filename;
        }

        public IEnumerable<StoryInfo> GetStoryInfos()
        {
             var filter = new FileExtensionFilter(_settings);

            if (!filter.IsValid(_filename))
                return new StoryInfo[0];

            return new [] {new StoryInfo {Location = _filename, Text = _filesystem.GetFileText(_filename)}};
        }
    }

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