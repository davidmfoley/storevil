using System;
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
        private FileExtensionFilter _filter;

        public FilesystemStoryReader(IFilesystem filesystem, ConfigSettings settings)
        {
            _settings = settings;
            Filesystem = filesystem;

            _filter = new FileExtensionFilter(_settings);
        }

        public IFilesystem Filesystem { get; set; }

        public IEnumerable<StoryInfo> GetStoryInfos()
        {
            return GetStoryInfos(_settings.StoryBasePath);
        }

        private IEnumerable<StoryInfo> GetStoryInfos(string path)
        {
            if (Filesystem.FileExists(path) && _filter.IsValid(path))
                return new[] {GetStoryInfo(path)};

            var filesMatchingFilter = GetFilesMatchingFilter(path);

            var stories = filesMatchingFilter
                .Select(GetStoryInfo)                       
                .ToList();

            foreach (var subPath in Filesystem.GetSubFolders(path))
                stories.AddRange(GetStoryInfos(subPath));

            return stories;
        }

        private IEnumerable<string> GetFilesMatchingFilter(string path)
        {
            return Filesystem
                .GetFilesInFolder(path)
                .Where(_filter.IsValid);
        }

        private StoryInfo GetStoryInfo(string file)
        {
            return new StoryInfo
                       {
                           Location = file,
                           Text = Filesystem.GetFileText(file)
                       };
        }
    }
}