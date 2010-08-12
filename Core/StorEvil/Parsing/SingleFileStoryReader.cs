using System.Collections.Generic;
using StorEvil.Configuration;
using StorEvil.Infrastructure;

namespace StorEvil.Parsing
{
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
}