using System.Collections.Generic;
using System.IO;
using System.Linq;
using StorEvil.Core;

namespace StorEvil.Parsing
{
    public class StoryProvider : IStoryProvider
    {
        private readonly IStoryReader _reader;
        private readonly IStoryParser _parser;

        public StoryProvider(IStoryReader reader, IStoryParser parser)
        {
            _reader = reader;
            _parser = parser;
        }

        public IEnumerable<Story> GetStories()
        {
            Story story;
            var stories = new List<Story>();

            foreach (var storyInfo in _reader.GetStoryInfos())
            {
                if (null == (story = _parser.Parse(storyInfo.Text, storyInfo.Id)))
                    continue;

                story.Id = storyInfo.Id;
                stories.Add(story);
            }
            return stories;
        }
    }

    public interface IStoryReader
    {
        IEnumerable<StoryInfo> GetStoryInfos();
    }

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
            var stories = Filesystem
                .GetFilesInFolder(path)
                .Where(ExtensionIsSupportedByCurrentSettings)
                .Select(file =>
                        new StoryInfo
                            {
                                Id = Path.GetFileNameWithoutExtension(file),
                                Text = Filesystem.GetFileText(file)
                            })
                .ToList();

            foreach (var subPath in Filesystem.GetSubFolders(path))
                stories.AddRange(GetStoryInfos(subPath));

            return stories;
        }

        private bool ExtensionIsSupportedByCurrentSettings(string file)
        {
            if (Settings.ScenarioExtensions == null || ! Settings.ScenarioExtensions.Any())
                return true;

            var extension = Path.GetExtension(file);
            return Settings.ScenarioExtensions.Any(x => extension == x);
        }
    }

    public class StoryInfo
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}